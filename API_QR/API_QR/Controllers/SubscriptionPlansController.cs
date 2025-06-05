using API_QR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Newtonsoft.Json;


namespace API_QR.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly SmartQrdineOptimizedContext _context;

        public SubscriptionsController(SmartQrdineOptimizedContext context)
        {
            _context = context;
        }


        //Lấy ra tất cả các gói 
        [HttpGet("plans")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubscriptionPlans()
        {
            try
            {
                var plans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive.HasValue)
                    .OrderBy(p => p.Price)
                    .Select(p => new
                    {
                        p.PlanId,
                        p.PlanName,
                        p.Description,
                        p.Price,
                        p.BillingCycle,
                        p.MaxTables,
                        p.MaxMenuItems,
                        p.IsActive,
                        p.Features,
                        p.CreatedAt
                    })
                    .ToListAsync();

                return Ok(plans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }


        //Lấy ra gói theo id
        [HttpGet("plans/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubscriptionPlan(int id)
        {
            try
            {
                var plan = await _context.SubscriptionPlans.FindAsync(id);
                if (plan == null || plan.IsActive != true)
                {
                    return NotFound(new { message = "Không tìm thấy gói đăng ký" });
                }


                return Ok(new
                {
                    plan.PlanId,
                    plan.PlanName,
                    plan.Description,
                    plan.Price,
                    plan.BillingCycle,
                    plan.MaxTables,
                    plan.MaxMenuItems,
                    plan.Features,

                    plan.CreatedAt,
                    plan.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }


        //Tạo 1 gói đăng ký mới
        [HttpPost("plans")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanRequest request)
        {
            try
            {
                var plan = new SubscriptionPlan
                {
                    PlanName = request.PlanName,
                    Description = request.Description,
                    Price = request.Price,
                    BillingCycle = request.BillingCycle,
                    MaxTables = request.MaxTables,
                    MaxMenuItems = request.MaxMenuItems,
                    Features = JsonConvert.SerializeObject(request.Features),
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.SubscriptionPlans.Add(plan);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSubscriptionPlan), new { id = plan.PlanId },
                    new { message = "Tạo gói đăng ký thành công", planId = plan.PlanId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }
        [HttpPut("plans/{id}")]
        public async Task<IActionResult> UpdateSubscriptionPlan(int id, [FromBody] CreateSubscriptionPlanRequest request)
        {
            try
            {
                var plan = await _context.SubscriptionPlans.FindAsync(id);
                if (plan == null)
                {
                    return NotFound(new { message = "Không tìm thấy gói đăng ký" });
                }

                plan.PlanName = request.PlanName;
                plan.Description = request.Description;
                plan.Price = request.Price;
                plan.BillingCycle = request.BillingCycle;
                plan.MaxTables = request.MaxTables;
                plan.MaxMenuItems = request.MaxMenuItems;
                // Properly serialize the features array
                plan.Features = JsonConvert.SerializeObject(
                request.Features,
                new JsonSerializerSettings
                {
                StringEscapeHandling = StringEscapeHandling.Default
                });

                plan.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật gói đăng ký thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }




        //Lấy ra tất cả các subscription
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSubscriptions()
        {
            try
            {
                var accessibleRestaurants = await GetAccessibleRestaurants();

                var subscriptions = await _context.Subscriptions
                    .Where(s => accessibleRestaurants.Contains(s.RestaurantId))
                    .Include(s => s.Restaurant)
                    .Include(s => s.Plan)
                    .Select(s => new
                    {
                        s.SubscriptionId,
                        s.RestaurantId,
                        RestaurantName = s.Restaurant.Name,
                        s.PlanId,
                        PlanName = s.Plan.PlanName,
                        s.StartDate,
                        s.EndDate,
                        s.Status,
                        s.AutoRenew,
                        s.CreatedAt,
                        s.UpdatedAt
                    })
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscription(int id)
        {
            try
            {
                var subscription = await _context.Subscriptions
                    .Include(s => s.Restaurant)
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s => s.SubscriptionId == id);

                if (subscription == null)
                {
                    return NotFound(new { message = "Không tìm thấy subscription" });
                }

                if (!await HasRestaurantAccess(subscription.RestaurantId))
                {
                    return Forbid();
                }

                // Get payment history
                var payments = await _context.Payments
                    .Where(p => p.SubscriptionId == id)
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => new
                    {
                        p.PaymentId,
                        p.Amount,
                        p.Currency,
                        p.PaymentMethod,
                        p.PaymentStatus,
                        p.PaymentDate,
                        p.TransactionId
                    })
                    .ToListAsync();

                return Ok(new
                {
                    subscription.SubscriptionId,
                    subscription.RestaurantId,
                    RestaurantInfo = new
                    {
                        subscription.Restaurant.RestaurantId,
                        subscription.Restaurant.Name,
                        subscription.Restaurant.Email,
                        subscription.Restaurant.PhoneNumber
                    },
                    subscription.PlanId,
                    PlanInfo = new
                    {
                        subscription.Plan.PlanId,
                        subscription.Plan.PlanName,
                        subscription.Plan.Description,
                        subscription.Plan.Price,
                        subscription.Plan.BillingCycle,
                        subscription.Plan.MaxTables,
                        subscription.Plan.MaxMenuItems,
                        subscription.Plan.Features
                    },
                    subscription.StartDate,
                    subscription.EndDate,
                    subscription.Status,
                    subscription.AutoRenew,
                    subscription.CreatedAt,
                    subscription.UpdatedAt,
                    DaysRemaining = subscription.EndDate > DateTime.Now ? (subscription.EndDate - DateTime.Now).Days : 0,
                    IsExpired = subscription.EndDate <= DateTime.Now,
                    PaymentHistory = payments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }



        //Đăng kí 1 subscription mới
        [HttpPost]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Validate restaurant access
                if (userRole != "Admin" && !await HasRestaurantAccess(request.RestaurantID))
                {
                    return Forbid();
                }

                // Validate plan
                var plan = await _context.SubscriptionPlans.FindAsync(request.PlanID);
                if (plan == null || !plan.IsActive.HasValue)
                {
                    return BadRequest(new { message = "Gói đăng ký không tồn tại hoặc không hoạt động" });
                }

                // Check for existing active subscription
                var existingSubscription = await _context.Subscriptions
                    .FirstOrDefaultAsync(s => s.RestaurantId == request.RestaurantID && s.Status == "Active");

                if (existingSubscription != null)
                {
                    return BadRequest(new { message = "Nhà hàng đã có subscription đang hoạt động" });
                }

                // Calculate end date based on billing cycle
                var endDate = request.StartDate;
                switch (plan.BillingCycle.ToLower())
                {
                    case "monthly":
                        endDate = request.StartDate.AddMonths(1);
                        break;
                    case "yearly":
                        endDate = request.StartDate.AddYears(1);
                        break;
                    default:
                        endDate = request.StartDate.AddMonths(1);
                        break;
                }

                var subscription = new Subscription
                {
                    RestaurantId = request.RestaurantID,
                    PlanId = request.PlanID,
                    StartDate = request.StartDate,
                    EndDate = endDate,
                    Status = "Pending", // Will be activated when payment is completed
                    AutoRenew = request.AutoRenew,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                // Update restaurant plan
                var restaurant = await _context.Restaurants.FindAsync(request.RestaurantID);
                if (restaurant != null)
                {
                    restaurant.PlanId = request.PlanID;
                    restaurant.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(GetSubscription), new { id = subscription.SubscriptionId },
                    new { message = "Tạo subscription thành công", subscriptionId = subscription.SubscriptionId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubscriptionStatus(int id, [FromBody] UpdateSubscriptionStatusRequest request)
        {
            try
            {
                var subscription = await _context.Subscriptions.FindAsync(id);
                if (subscription == null)
                {
                    return NotFound(new { message = "Không tìm thấy subscription" });
                }

                subscription.Status = request.Status;
                subscription.UpdatedAt = DateTime.Now;

                if (request.Status == "Cancelled" || request.Status == "Expired")
                {
                    subscription.AutoRenew = false;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật trạng thái subscription thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpPut("{id}/auto-renew")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<IActionResult> UpdateAutoRenew(int id, [FromBody] UpdateAutoRenewRequest request)
        {
            try
            {
                var subscription = await _context.Subscriptions.FindAsync(id);
                if (subscription == null)
                {
                    return NotFound(new { message = "Không tìm thấy subscription" });
                }

                if (!await HasRestaurantAccess(subscription.RestaurantId))
                {
                    return Forbid();
                }

                subscription.AutoRenew = request.AutoRenew;
                subscription.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật auto-renew thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpPost("{id}/renew")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<IActionResult> RenewSubscription(int id)
        {
            try
            {
                var subscription = await _context.Subscriptions
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s => s.SubscriptionId == id);

                if (subscription == null)
                {
                    return NotFound(new { message = "Không tìm thấy subscription" });
                }

                if (!await HasRestaurantAccess(subscription.RestaurantId))
                {
                    return Forbid();
                }

                // Calculate new end date
                var newEndDate = subscription.EndDate;
                switch (subscription.Plan.BillingCycle.ToLower())
                {
                    case "monthly":
                        newEndDate = subscription.EndDate.AddMonths(1);
                        break;
                    case "yearly":
                        newEndDate = subscription.EndDate.AddYears(1);
                        break;
                    default:
                        newEndDate = subscription.EndDate.AddMonths(1);
                        break;
                }

                subscription.EndDate = newEndDate;
                subscription.Status = "Active";
                subscription.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Gia hạn subscription thành công", newEndDate });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSubscriptionStatistics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                startDate ??= DateTime.Today.AddDays(-30);
                endDate ??= DateTime.Today.AddDays(1);

                var subscriptions = await _context.Subscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.CreatedAt >= startDate && s.CreatedAt < endDate)
                    .ToListAsync();

                var allSubscriptions = await _context.Subscriptions
                    .Include(s => s.Plan)
                    .ToListAsync();

                var statistics = new
                {
                    TotalSubscriptions = allSubscriptions.Count,
                    ActiveSubscriptions = allSubscriptions.Count(s => s.Status == "Active"),
                    ExpiredSubscriptions = allSubscriptions.Count(s => s.Status == "Expired"),
                    CancelledSubscriptions = allSubscriptions.Count(s => s.Status == "Cancelled"),
                    NewSubscriptions = subscriptions.Count,
                    TotalRevenue = await _context.Payments
                        .Where(p => p.PaymentType == "Subscription" && p.PaymentStatus == "Completed" &&
                               p.CreatedAt >= startDate && p.CreatedAt < endDate)
                        .SumAsync(p => p.Amount),
                    SubscriptionsByPlan = allSubscriptions
                        .Where(s => s.Status == "Active")
                        .GroupBy(s => s.Plan.PlanName)
                        .Select(g => new { PlanName = g.Key, Count = g.Count() })
                        .ToList(),
                    ExpiringSubscriptions = allSubscriptions
                        .Where(s => s.Status == "Active" && s.EndDate <= DateTime.Now.AddDays(7))
                        .Count(),
                    AutoRenewEnabled = allSubscriptions.Count(s => s.AutoRenew.HasValue),
                    DailySubscriptions = subscriptions
                        .GroupBy(s => s.CreatedAt.GetValueOrDefault())
                        .Select(g => new { Date = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Date)
                        .ToList()
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        private async Task<bool> HasRestaurantAccess(int restaurantId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Admin")
                return true;

            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant?.OwnerUserId == currentUserId)
                return true;

            return await _context.RestaurantStaffs
                .AnyAsync(s => s.RestaurantId == restaurantId && s.UserId == currentUserId);
        }


        
        private async Task<List<int>> GetAccessibleRestaurants()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Admin")
            {
                return await _context.Restaurants.Select(r => r.RestaurantId).ToListAsync();
            }

            var ownedRestaurants = await _context.Restaurants
                .Where(r => r.OwnerUserId == currentUserId)
                .Select(r => r.RestaurantId)
                .ToListAsync();

            var staffRestaurants = await _context.RestaurantStaffs
                .Where(s => s.UserId == currentUserId)
                .Select(s => s.RestaurantId)
                .ToListAsync();

            return ownedRestaurants.Union(staffRestaurants).ToList();
        }
    }

    public class CreateSubscriptionPlanRequest
    {
        public string PlanName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public int? MaxTables { get; set; }
        public int? MaxMenuItems { get; set; }
        public List<string> Features { get; set; } = new();
    }

    public class CreateSubscriptionRequest
    {
        public int RestaurantID { get; set; }
        public int PlanID { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public bool AutoRenew { get; set; } = true;
    }

    public class UpdateSubscriptionStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateAutoRenewRequest
    {
        public bool AutoRenew { get; set; }
    }


}
