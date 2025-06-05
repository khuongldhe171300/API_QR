using API_QR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SkiaSharp;


using System.Security.Claims;

namespace API_QR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TablesController : ControllerBase
    {
        private readonly SmartQrdineOptimizedContext _context;

        public TablesController(SmartQrdineOptimizedContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTables()
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Không thể xác định người dùng.");
            }

            var restaurants = await _context.Restaurants
                .Where(r => r.OwnerUserId == userId)
                .Include(u => u.OwnerUser)
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync();
            try
            {
                if (!await HasRestaurantAccess(restaurants.RestaurantId))
                {
                    return Forbid();
                }

                var tables = await _context.RestaurantTables
                    .Where(t => t.RestaurantId == restaurants.RestaurantId)
                    .OrderBy(t => t.TableNumber)
                    .Select(t => new
                    {
                        t.TableId,
                        t.TableNumber,
                        t.Capacity,
                        t.Location,
                        t.Status,
                        t.QrcodeUrl,
                        t.QrcodeData,
                        t.CreatedAt,
                        t.UpdatedAt,
                        CurrentOrder = _context.Orders
                            .Where(o => o.TableId == t.TableId &&
                                (o.Status == "Pending" || o.Status == "Confirmed" || o.Status == "Preparing" || o.Status == "Ready"))
                            .Select(o => new { o.OrderId, o.CustomerName, o.Status, o.CreatedAt })
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(tables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }


        /// Lấy thông tin bàn theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTable(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Không thể xác định người dùng.");
                }

                var restaurants = await _context.Restaurants
                    .Where(r => r.OwnerUserId == userId)
                    .Include(u => u.OwnerUser)
                    .Include(u => u.Subscriptions)
                    .FirstOrDefaultAsync();

                var table = await _context.RestaurantTables
                    .Include(t => t.Restaurant)
                    .FirstOrDefaultAsync(t => t.TableId == id);

                if (table == null)
                {
                    return NotFound(new { message = "Không tìm thấy bàn" });
                }

                if (!await HasRestaurantAccess(restaurants.RestaurantId))
                {
                    return Forbid();
                }

                var currentOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Item)
                    .Where(o => o.TableId == id &&
                        (o.Status == "Pending" || o.Status == "Confirmed" || o.Status == "Preparing" || o.Status == "Ready"))
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    table.TableId,
                    table.RestaurantId,
                    RestaurantName = table.Restaurant.Name,
                    table.TableNumber,
                    table.Capacity,
                    table.Location,
                    table.Status,
                    table.QrcodeUrl,
                    table.QrcodeData,
                    table.CreatedAt,
                    table.UpdatedAt,
                    CurrentOrder = currentOrder != null ? new
                    {
                        currentOrder.OrderId,
                        currentOrder.CustomerName,
                        currentOrder.CustomerPhone,
                        currentOrder.Status,
                        currentOrder.TotalAmount,
                        currentOrder.NetAmount,
                        currentOrder.CreatedAt,
                        ItemCount = currentOrder.OrderItems.Count,
                        Items = currentOrder.OrderItems.Select(oi => new
                        {
                            oi.OrderItemId,
                            oi.Item.ItemId,
                            oi.Quantity,
                            oi.UnitPrice,
                            oi.Subtotal,
                            oi.Status
                        }).ToList()
                    } : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }


        ////hàm tạo qr
        private string GenerateQrCodeWithSkia(string data, string fileName)
        {
            using var generator = new QRCodeGenerator();
            using var qrData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            // Render thủ công từ matrix
            int pixelsPerModule = 10;
            var matrix = qrData.ModuleMatrix;
            int size = matrix.Count * pixelsPerModule;

            using var bitmap = new SKBitmap(size, size);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Fill
            };

            for (int y = 0; y < matrix.Count; y++)
            {
                for (int x = 0; x < matrix[y].Count; x++)
                {
                    if (matrix[y][x])
                    {
                        canvas.DrawRect(x * pixelsPerModule, y * pixelsPerModule, pixelsPerModule, pixelsPerModule, paint);
                    }
                }
            }

            // Lưu vào wwwroot/qrcodes
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, fileName);

            using var fs = System.IO.File.OpenWrite(path);
            bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);

            return $"/qrcodes/{fileName}";
        }

        // Trả về file ảnh QR từ byte[]
        [HttpGet("qr-preview/{tableId}")]
        public IActionResult GetQrPreview(int tableId)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes("wwwroot/qrcodes/restaurant-1-table-E5.png");
            return File(fileBytes, "image/png"); // ✅ Có 'return'
        }



        [HttpPost("qr-code-create/{tbId}")]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> CreateQr([FromForm]string tbId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Không thể xác định người dùng.");
                }

                var restaurants = await _context.Restaurants
                    .Where(r => r.OwnerUserId == userId)
                    .Include(u => u.OwnerUser)
                    .Include(u => u.Subscriptions)
                    .FirstOrDefaultAsync();

                var table = await _context.RestaurantTables
                    .FirstOrDefaultAsync(t => t.RestaurantId == restaurants.RestaurantId && t.TableNumber == tbId);

                if (table == null)
                {
                    return NotFound(new { message = "Không tìm thấy bàn với số bàn đã cho." });
                }               

                var qrCodeData = $"http://localhost:3000/order/{restaurants.RestaurantId}/{table.TableNumber}";
                var fileName = $"restaurant-{restaurants.RestaurantId}-table-{table.TableNumber}.png";

                // 🟢 Gọi hàm tạo QR Code thật sự
                var qrCodePath = GenerateQrCodeWithSkia(qrCodeData, fileName);

                if (string.IsNullOrEmpty(qrCodePath))
                {
                    return BadRequest(new { message = "Không thể tạo QR Code" });
                }            
                // Cập nhật thông tin bàn               
                table.QrcodeData = qrCodeData;
                table.QrcodeUrl = qrCodePath;
                table.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // Trả về thông tin bàn đã cập nhật

               
                return CreatedAtAction(nameof(GetTable), new { id = table.TableId },
                    new { message = "Thêm qr thành công", tableId = table.TableId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> CreateTable([FromForm] CreateTableRequest request)
        {
            try
            { 
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Không thể xác định người dùng.");
                }

                var restaurants = await _context.Restaurants
                    .Where(r => r.OwnerUserId == userId)
                    .Include(u => u.OwnerUser)
                    .Include(u => u.Subscriptions)
                    .FirstOrDefaultAsync();

                if (!await HasRestaurantAccess(restaurants.RestaurantId))
                {
                    return Forbid();
                }

                var existingTable = await _context.RestaurantTables
                    .AnyAsync(t => t.RestaurantId == restaurants.RestaurantId && t.TableNumber == request.TableNumber);

                if (existingTable)
                {
                    return BadRequest(new { message = "Số bàn đã tồn tại" });
                }

              

                var table = new RestaurantTable
                {
                    RestaurantId = restaurants.RestaurantId,
                    TableNumber = request.TableNumber,
                    Capacity = request.Capacity,
                    Location = request.Location,
                    Status = "Available",
                    CreatedAt = DateTime.Now,
                };

                _context.RestaurantTables.Add(table);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTable), new { id = table.TableId },
                    new { message = "Tạo bàn thành công", tableId = table.TableId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> UpdateTable(int id, [FromBody] UpdateTableRequest request)
        {
            try
            {
                var table = await _context.RestaurantTables.FindAsync(id);
                if (table == null)
                {
                    return NotFound(new { message = "Không tìm thấy bàn" });
                }

                if (!await HasRestaurantAccess(table.RestaurantId))
                {
                    return Forbid();
                }

                // Check if new table number already exists (if changing)
                if (!string.IsNullOrEmpty(request.TableNumber) && request.TableNumber != table.TableNumber)
                {
                    var existingTable = await _context.RestaurantTables
                        .AnyAsync(t => t.RestaurantId == table.RestaurantId && t.TableNumber == request.TableNumber);

                    if (existingTable)
                    {
                        return BadRequest(new { message = "Số bàn đã tồn tại" });
                    }

                    table.TableNumber = request.TableNumber;
                    table.QrcodeData = $"http://localhost:3000/order/{table.Restaurant.Name}/{request.TableNumber}";
                    table.QrcodeUrl = $"/qrcodes/restaurant-{table.RestaurantId}-table-{request.TableNumber}.png";
                }

                table.Capacity = request.Capacity ?? table.Capacity;
                table.Location = request.Location ?? table.Location;
                table.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật bàn thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> UpdateTableStatus(int id, [FromBody] UpdateTableStatusRequest request)
        {
            try
            {
                var table = await _context.RestaurantTables.FindAsync(id);
                if (table == null)
                {
                    return NotFound(new { message = "Không tìm thấy bàn" });
                }

                if (!await HasRestaurantAccess(table.RestaurantId))
                {
                    return Forbid();
                }

                table.Status = request.Status;
                table.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật trạng thái bàn thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            try
            {
                var table = await _context.RestaurantTables.FindAsync(id);
                if (table == null)
                {
                    return NotFound(new { message = "Không tìm thấy bàn" });
                }

                if (!await HasRestaurantAccess(table.RestaurantId))
                {
                    return Forbid();
                }

                // Check if table has active orders
                var hasActiveOrders = await _context.Orders
                    .AnyAsync(o => o.TableId == id &&
                        (o.Status == "Pending" || o.Status == "Confirmed" || o.Status == "Preparing" || o.Status == "Ready"));

                if (hasActiveOrders)
                {
                    return BadRequest(new { message = "Không thể xóa bàn đang có đơn hàng" });
                }

                _context.RestaurantTables.Remove(table);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa bàn thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpGet("{id}/qr-code")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTableQRCode(int id)
        {
            try
            {
                var table = await _context.RestaurantTables
                    .Include(t => t.Restaurant)
                    .FirstOrDefaultAsync(t => t.TableId == id);

                if (table == null)
                {
                    return NotFound(new { message = "Không tìm thấy bàn" });
                }

                return Ok(new
                {
                    table.TableId,
                    table.TableNumber,
                    RestaurantName = table.Restaurant.Name,
                    table.QrcodeData,
                    table.QrcodeUrl,
                    OrderURL = table.QrcodeData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpGet("restaurant/{restaurantId}/statistics")]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> GetTableStatistics(int restaurantId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                if (!await HasRestaurantAccess(restaurantId))
                {
                    return Forbid();
                }

                startDate ??= DateTime.Today.AddDays(-30);
                endDate ??= DateTime.Today.AddDays(1);

                var tables = await _context.RestaurantTables
                    .Where(t => t.RestaurantId == restaurantId)
                    .ToListAsync();

                var tableStats = new List<object>();

                foreach (var table in tables)
                {
                    var orders = await _context.Orders
                        .Where(o => o.TableId == table.TableId && o.CreatedAt >= startDate && o.CreatedAt < endDate)
                        .ToListAsync();

                    var completedOrders = orders.Where(o => o.Status == "Completed").ToList();

                    tableStats.Add(new
                    {
                        table.TableId,
                        table.TableNumber,
                        table.Capacity,
                        table.Location,
                        table.Status,
                        TotalOrders = orders.Count,
                        CompletedOrders = completedOrders.Count,
                        TotalRevenue = completedOrders.Sum(o => o.NetAmount),
                        AverageOrderValue = completedOrders.Any() ? completedOrders.Average(o => o.NetAmount) : 0,
                        UtilizationRate = orders.Count > 0 ? (double)completedOrders.Count / orders.Count * 100 : 0
                    });
                }

                var overallStats = new
                {
                    TotalTables = tables.Count,
                    AvailableTables = tables.Count(t => t.Status == "Available"),
                    OccupiedTables = tables.Count(t => t.Status == "Occupied"),
                    ReservedTables = tables.Count(t => t.Status == "Reserved"),
                    OutOfServiceTables = tables.Count(t => t.Status == "Out of Service"),
                    TotalCapacity = tables.Sum(t => t.Capacity ?? 0),
                    AverageUtilization = tableStats.Any() ? tableStats.Average(t => (double)((dynamic)t).UtilizationRate) : 0
                };

                return Ok(new
                {
                    OverallStats = overallStats,
                    TableStats = tableStats.OrderByDescending(t => ((dynamic)t).TotalRevenue)
                });
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
    }

    public class CreateTableRequest
    {
        public string TableNumber { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public string? Location { get; set; }

        public string? Status { get; set; }
    }

    public class UpdateTableRequest
    {
        public string? TableNumber { get; set; }
        public int? Capacity { get; set; }
        public string? Location { get; set; }
    }


    public class CreateQRCodeRequestDto
    {
        public int tableId { get; set; }
        public string QrcodeData { get; set; } = string.Empty;
    }

    public class UpdateTableStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
