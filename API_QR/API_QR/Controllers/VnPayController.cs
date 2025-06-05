using API_QR.Helpers;
using API_QR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_QR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VnPayController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SmartQrdineOptimizedContext _context;
        private readonly ILogger<VnPayController> _logger;

        public VnPayController(IConfiguration config, SmartQrdineOptimizedContext context, ILogger<VnPayController> logger)
        {
            _config = config;
            _context = context;
            _logger = logger;
        }

        [HttpPost("create-subscription-payment")]
        public IActionResult CreateSubscriptionPayment([FromBody] int planId)
        {
            try
            {
                _logger.LogInformation($"Bắt đầu tạo thanh toán subscription cho planId: {planId}");

                // Kiểm tra user đã đăng nhập chưa
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("Không thể xác định người dùng");
                    return Unauthorized("Không thể xác định người dùng.");
                }

                var restaurant = _context.Restaurants
                    .Where(r => r.OwnerUserId == userId)
                    .Include(r => r.OwnerUser)
                    .Include(r => r.Subscriptions)
                    .FirstOrDefault();

                if (restaurant == null)
                {
                    _logger.LogWarning($"Không tìm thấy nhà hàng cho userId: {userId}");
                    return NotFound("Không tìm thấy nhà hàng của người dùng.");
                }

                var plan = _context.SubscriptionPlans.Find(planId);
                if (plan == null)
                {
                    _logger.LogWarning($"Không tìm thấy gói subscription với id: {planId}");
                    return NotFound(new { message = "Không tìm thấy gói" });
                }

                // Lấy thông tin cấu hình VnPay
                var tmnCode = _config["VnPay:TmnCode"];
                var hashSecret = _config["VnPay:HashSecret"];
                var paymentUrl = _config["VnPay:Url"];
                var returnUrl = _config["VnPay:ReturnUrl"];

                if (string.IsNullOrEmpty(tmnCode) || string.IsNullOrEmpty(hashSecret) ||
                    string.IsNullOrEmpty(paymentUrl) || string.IsNullOrEmpty(returnUrl))
                {
                    _logger.LogError("Thiếu cấu hình VnPay trong appsettings.json");
                    return StatusCode(500, new { message = "Lỗi cấu hình thanh toán" });
                }

                // Tạo link thanh toán VNPAY
                var vnPay = new VnPayLibrary();

                // Lấy IP address
                var ip = GetClientIpAddress();

                _logger.LogInformation($"Tạo thanh toán VnPay cho nhà hàng: {restaurant.Name}, IP: {ip}");

                // Thêm các tham số theo đúng thứ tự alphabet
                vnPay.AddRequestData("vnp_Amount", ((long)(plan.Price * 100)).ToString());
                vnPay.AddRequestData("vnp_Command", "pay");
                vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnPay.AddRequestData("vnp_CurrCode", "VND");
                vnPay.AddRequestData("vnp_IpAddr", ip);
                vnPay.AddRequestData("vnp_Locale", "vn");
                vnPay.AddRequestData("vnp_OrderInfo", "Thanh toan dich vu");
                vnPay.AddRequestData("vnp_OrderType", "other");
                vnPay.AddRequestData("vnp_ReturnUrl", returnUrl);
                vnPay.AddRequestData("vnp_TmnCode", tmnCode);
                vnPay.AddRequestData("vnp_TxnRef", $"SUB_{userId}_{planId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");
                vnPay.AddRequestData("vnp_Version", "2.1.0");

                var paymentUrlWithParams = vnPay.CreateRequestUrl(paymentUrl, hashSecret);

                _logger.LogInformation($"URL thanh toán đã tạo: {paymentUrlWithParams}");

                return Ok(new { url = paymentUrlWithParams });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thanh toán VnPay");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý thanh toán" });
            }
        }

        [HttpGet("payment-return")]
        public async Task<IActionResult> PaymentReturn()
        {
            try
            {
                var query = Request.Query;
                _logger.LogInformation($"Nhận được callback từ VnPay: {string.Join(", ", query.Select(q => $"{q.Key}={q.Value}"))}");

                var vnpSecureHash = query["vnp_SecureHash"].ToString();
                var vnpHashSecret = _config["VnPay:HashSecret"];

                // Sử dụng VnPayLibrary để validate signature
                var vnpay = new VnPayLibrary();

                foreach (var (key, value) in query)
                {
                    if (!string.IsNullOrEmpty(value) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value);
                    }
                }

                // Kiểm tra hash với 2 tham số đúng như method signature
                var checkSignature = vnpay.ValidateSignature(vnpSecureHash, vnpHashSecret);
                if (!checkSignature)
                {
                    _logger.LogWarning("Chữ ký không hợp lệ từ VnPay");
                    return Redirect($"{_config["Frontend:Url"]}/payment/callback?vnp_ResponseCode=97");
                }

                var txnRef = query["vnp_TxnRef"].ToString();
                var responseCode = query["vnp_ResponseCode"].ToString();

                _logger.LogInformation($"Mã giao dịch: {txnRef}, Mã phản hồi: {responseCode}");

                // Chuyển hướng về frontend với tất cả thông tin từ VnPay
                var callbackUrl = $"{_config["Frontend:Url"]}/payment/callback?" + string.Join("&", query.Select(q => $"{q.Key}={q.Value}"));

                if (responseCode == "00" && txnRef.StartsWith("SUB_"))
                {
                    // Phân tích txnRef để lấy userId và planId
                    var parts = txnRef.Split('_');
                    if (parts.Length == 4 &&
                        int.TryParse(parts[1], out int userId) &&
                        int.TryParse(parts[2], out int planId))
                    {
                        var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerUserId == userId);
                        var plan = await _context.SubscriptionPlans.FindAsync(planId);

                        if (restaurant != null && plan != null)
                        {
                            _logger.LogInformation($"Thanh toán thành công cho nhà hàng: {restaurant.Name}, gói: {plan.PlanName}");

                            // Kiểm tra xem đã có subscription chưa để tránh tạo trùng
                            var existingSubscription = await _context.Subscriptions
                                .FirstOrDefaultAsync(s => s.RestaurantId == restaurant.RestaurantId && s.Status == "Active");

                            if (existingSubscription == null)
                            {
                                // Thanh toán thành công mới tạo Subscription và Payment
                                var subscription = new Subscription
                                {
                                    RestaurantId = restaurant.RestaurantId,
                                    PlanId = plan.PlanId,
                                    StartDate = DateTime.Now,
                                    EndDate = DateTime.Now.AddMonths(1),
                                    Status = "Active",
                                    AutoRenew = true,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now
                                };

                                _context.Subscriptions.Add(subscription);

                                _context.Payments.Add(new Payment
                                {
                                    PaymentType = "Subscription",
                                    Subscription = subscription,
                                    Amount = plan.Price,
                                    Currency = "VND",
                                    PaymentMethod = "VNPAY",
                                    PaymentStatus = "Completed",
                                    PaymentDate = DateTime.Now,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now
                                });

                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Đã lưu thông tin subscription và payment vào database");
                            }
                        }
                    }
                }

                return Redirect(callbackUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý callback từ VnPay");
                return Redirect($"{_config["Frontend:Url"]}/payment/callback?vnp_ResponseCode=99");
            }
        }

        private string GetClientIpAddress()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Xử lý các trường hợp đặc biệt
            if (string.IsNullOrEmpty(ip) || ip == "::1")
            {
                return "127.0.0.1";
            }

            // Nếu là IPv6, chuyển về IPv4 nếu có thể
            if (ip.Contains("::ffff:"))
            {
                ip = ip.Replace("::ffff:", "");
            }

            return ip;
        }
    }
}
