using API_QR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_QR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly SmartQrdineOptimizedContext _context;

        public AdminController(SmartQrdineOptimizedContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            // Tổng số user
            var totalUsers = await _context.Users.CountAsync();

            // User online hôm nay
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            var onlineToday = await _context.Users.CountAsync(u => u.LastLogin >= today && u.LastLogin < tomorrow);

            // User online hôm qua
            var yesterday = today.AddDays(-1);
            var onlineYesterday = await _context.Users.CountAsync(u => u.LastLogin >= yesterday && u.LastLogin < today);

            // Tăng trưởng tháng
            var now = DateTime.UtcNow.AddHours(7); // Giờ Việt Nam
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var currentMonthCount = await _context.Users.CountAsync(u => u.CreatedAt >= currentMonthStart && u.CreatedAt < currentMonthStart.AddMonths(1));
            var previousMonthCount = await _context.Users.CountAsync(u => u.CreatedAt >= previousMonthStart && u.CreatedAt < currentMonthStart);
            double growthPercent = previousMonthCount == 0
                ? 100
                : ((currentMonthCount - previousMonthCount) / (double)previousMonthCount) * 100;

            return Ok(new
            {
                totalUsers,
                onlineToday,
                onlineYesterday,
                monthlyGrowth = Math.Round(growthPercent, 2)
            });
        }



    }
}
