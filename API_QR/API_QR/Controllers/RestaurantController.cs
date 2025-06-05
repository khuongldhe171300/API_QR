using API_QR.DTOs;
using API_QR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_QR.Controllers
{
    // RestaurantController.cs
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,RestaurantOwner")]
    public class RestaurantController : ControllerBase
    {
        private readonly SmartQrdineOptimizedContext _context;

        public RestaurantController(SmartQrdineOptimizedContext context)
        {
            _context = context;
        }

        // ✅ Get all restaurants (Admin only)
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Restaurants.Include(u => u.OwnerUser)
                .ToListAsync();
            var userDtos = users.Select(u => new
            {
                u.RestaurantId,
                u.Name,
                u.Description,
                u.Address,
                u.City,
                u.State,
                u.Country,
                u.PostalCode,
                u.PhoneNumber,
                u.Email,
                u.Website,
                u.LogoUrl,
                u.CoverImageUrl,
                OwernerUser = u.OwnerUser.FirstName + " " + u.OwnerUser.LastName,
                u.Subscriptions,
                u.IsActive,
                u.Language,
                u.Currency,
                u.TaxRate,
                u.ServiceChargeRate,
                u.CreatedAt,

                u.UpdatedAt
            }).ToList();


            return Ok(userDtos);
        }

        [HttpGet("my-restaurants")] // hoặc [HttpGet] nếu bạn muốn
        public async Task<IActionResult> GetById()
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
                .ToListAsync();

            var restaurantDtos = restaurants.Select(u => new
            {
                u.RestaurantId,
                u.Name,
                u.Description,
                u.Address,
                u.City,
                u.State,
                u.Country,
                u.PostalCode,
                u.PhoneNumber,
                u.Email,
                u.Website,
                u.LogoUrl,
                u.CoverImageUrl,
                OwnerUser = u.OwnerUser.FirstName + " " + u.OwnerUser.LastName,
                u.Subscriptions,
                u.IsActive,
                u.Language,
                u.Currency,
                u.TaxRate,
                u.ServiceChargeRate,
                u.CreatedAt,
                u.UpdatedAt
            }).ToList();

            return Ok(restaurantDtos);
        }



        // ✅ Create restaurant (Admin or Owner)
        [HttpPost]
        public async Task<IActionResult> Create(RestaurantDto dto)
        {
            var restaurant = new Restaurant
            {
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Website = dto.Website,
                LogoUrl = dto.LogoURL,
                CoverImageUrl = dto.CoverImageURL,
                OwnerUserId = dto.OwnerUserID,
                PlanId = dto.PlanID,
                Language = dto.Language,
                Currency = dto.Currency,
                TaxRate = dto.TaxRate,
                ServiceChargeRate = dto.ServiceChargeRate,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            return Ok(restaurant);
        }

        // ✅ Update restaurant
        [HttpPut()]
        public async Task<IActionResult> Update(RestaurantDto dto)

        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Không thể xác định người dùng.");
            }

            var restaurant = await _context.Restaurants
                .Where(r => r.OwnerUserId == userId)
                .Include(u => u.OwnerUser)
                .Include(u => u.Subscriptions)
.FirstOrDefaultAsync();
            if (restaurant == null) return NotFound();

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.Address = dto.Address;
            restaurant.City = dto.City;
            restaurant.State = dto.State;
            restaurant.Country = dto.Country;
            restaurant.PostalCode = dto.PostalCode;
            restaurant.PhoneNumber = dto.PhoneNumber;
            restaurant.Email = dto.Email;
            restaurant.Website = dto.Website;
            restaurant.LogoUrl = dto.LogoURL;
            restaurant.CoverImageUrl = dto.CoverImageURL;
            restaurant.PlanId = dto.PlanID;
            restaurant.Language = dto.Language;
            restaurant.Currency = dto.Currency;
            restaurant.TaxRate = dto.TaxRate;
            restaurant.ServiceChargeRate = dto.ServiceChargeRate;
            restaurant.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(restaurant);
        }

        // ✅ Delete restaurant
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null) return NotFound();

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
            return Ok("Đã xóa nhà hàng.");
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings(RestaurantSettingsDto dto)


        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Không thể xác định người dùng.");
            }

            var restaurant = await _context.Restaurants
                .Where(r => r.OwnerUserId == userId)
                .Include(u => u.OwnerUser)
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync();
            if (restaurant == null) return NotFound();

            restaurant.Language = dto.Language;
            restaurant.Currency = dto.Currency;
            restaurant.TaxRate = dto.TaxRate;
            restaurant.ServiceChargeRate = dto.ServiceChargeRate;
            restaurant.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok("Đã cập nhật cài đặt nhà hàng.");
        }


        [HttpGet("stats-today")]
        [Authorize(Roles = "RestaurantOwner, Staff")]

        public async Task<IActionResult> GetTodayStats()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Không thể xác định người dùng.");
            }

            var restaurant = await _context.Restaurants
                .Where(r => r.OwnerUserId == userId)
                .Include(u => u.OwnerUser)
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync();
            if (restaurant == null) return NotFound();

            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            var yesterday = today.AddDays(-1);

            // ĐƠN HÀNG HÔM NAY
            var ordersToday = await _context.Orders
                .Where(o => o.RestaurantId == restaurant.RestaurantId && o.CreatedAt >= today && o.CreatedAt < tomorrow)
                .ToListAsync();
            var ordersYesterday = await _context.Orders
                .Where(o => o.RestaurantId == restaurant.RestaurantId && o.CreatedAt >= yesterday && o.CreatedAt < today)
                .ToListAsync();
            int countToday = ordersToday.Count;
            int countYesterday = ordersYesterday.Count;
            double ordersPercent = countYesterday == 0 ? 100 : ((countToday - countYesterday) / (double)countYesterday) * 100;

            // DOANH THU HÔM NAY
            decimal revenueToday = ordersToday.Sum(o => o.NetAmount );
            decimal revenueYesterday = ordersYesterday.Sum(o => o.NetAmount );
            double revenuePercent = revenueYesterday == 0 ? 100 : ((double)(revenueToday - revenueYesterday) / (double)revenueYesterday) * 100;

            // ĐƠN HÀNG THÀNH CÔNG (Status = "Completed")
            int successToday = ordersToday.Count(o => o.Status == "Completed");
            int successYesterday = ordersYesterday.Count(o => o.Status == "Completed");
            double successPercent = successYesterday == 0 ? 100 : ((successToday - successYesterday) / (double)successYesterday) * 100;

            // KHÁCH HÀNG THÂN THIẾT (vd: khách hàng có >=2 đơn hôm nay)
            var loyalToday = ordersToday.GroupBy(o => o.CustomerPhone)
                .Where(g => !string.IsNullOrEmpty(g.Key) && g.Count() >= 2)
                .Count();
            var loyalYesterday = ordersYesterday.GroupBy(o => o.CustomerPhone)
                .Where(g => !string.IsNullOrEmpty(g.Key) && g.Count() >= 2)
                .Count();
            double loyalPercent = loyalYesterday == 0 ? 100 : ((loyalToday - loyalYesterday) / (double)loyalYesterday) * 100;

            return Ok(new
            {
                ordersToday = countToday,
                ordersPercent = Math.Round(ordersPercent, 2),
                revenueToday = revenueToday,
                revenuePercent = Math.Round(revenuePercent, 2),
                successToday,
                successPercent = Math.Round(successPercent, 2),
                loyalToday,
                loyalPercent = Math.Round(loyalPercent, 2)
            });
        }

        [HttpGet("owner")]
        public async Task<IActionResult> GetByOwnerId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Không thể xác định người dùng.");
            }

            var restaurant = await _context.Restaurants
                .Where(r => r.OwnerUserId == userId)
                .Include(r => r.OwnerUser)
                .Include(r => r.Subscriptions)
                .FirstOrDefaultAsync();

            if (restaurant == null)
            {
                return NotFound(new { message = "Không tìm thấy nhà hàng." });
            }



            return Ok(new
            {
                restaurant.RestaurantId,
                restaurant.Name,
                restaurant.Email,
                restaurant.PhoneNumber,
                restaurant.Address,
                ownerUser = new
                {
                    restaurant.OwnerUser.UserId,
                    restaurant.OwnerUser.FirstName,
                    restaurant.OwnerUser.LastName,
                    restaurant.OwnerUser.Email
                }
            });


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

}
