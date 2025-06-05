using API_QR.DTOs;
using API_QR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_QR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SmartQrdineOptimizedContext _context;

        public UserController(SmartQrdineOptimizedContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized();

            if (!int.TryParse(claim.Value, out int userId))
                return Unauthorized("Invalid user ID in token.");

            var user = await _context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.UserId,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                Role = user.Role?.RoleName,
                user.EmailVerified,
                user.IsActive
            });
        }


        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrEmpty(dto.Email))
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserId != userId))
                    return BadRequest("Email đã được sử dụng.");

                user.Email = dto.Email;
                user.EmailVerified = dto.EmailVerified; 
            }
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok("Cập nhật thành công.");
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            var userDtos = users.Select(u => new
            {
                u.UserId,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                Role = u.Role?.RoleName,
                u.EmailVerified,
                u.IsActive
            });

            return Ok(userDtos);
        }
    }
}
