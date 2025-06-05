using API_QR.DTOs;
using API_QR.Helpers;
using API_QR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_QR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SmartQrdineOptimizedContext _context;
        private readonly JwtHelper _jwt;

        public AuthController(SmartQrdineOptimizedContext context, JwtHelper jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        [HttpPost("register-customer")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest req)
        {
            // 1. Kiểm tra email đã tồn tại
            if (await _context.Users.AnyAsync(u => u.Email == req.Email))
                return BadRequest(new { message = "Email đã được sử dụng" });

            // 2. Tạo user mới với mật khẩu đã mã hóa BCrypt
            var user = new User
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                FirstName = req.FirstName,
                LastName = req.LastName,
                PhoneNumber = req.PhoneNumber,
                RoleId = 2, // Giả sử 2 = Chủ nhà hàng, chỉnh nếu cần
                IsActive = true,
                EmailVerified = false,
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Để lấy UserID

            // 3. Tạo restaurant mới, gán OwnerUserID là user vừa tạo
            var restaurant = new Restaurant
            {
                Name = req.Name,
                Description = req.Description,
                Address = req.Address,
                City = req.City,
                State = req.State,
                Country = req.Country,
                PostalCode = req.PostalCode,
                PhoneNumber = req.RestaurantPhone,
                Email = req.RestaurantEmail,
                Website = req.Website,
                OwnerUserId = user.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                IsActive = true,
            };
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đăng ký thành công",
                userId = user.UserId,
                restaurantId = restaurant.RestaurantId
            });
        }
    

    [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Email hoặc mật khẩu không đúng.");

            var token = _jwt.GenerateToken(user, user.Role?.RoleName ?? "Customer");

            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new {
                token,
                userId = user.UserId,
                role = user.Role?.RoleName,
                fullName = $"{user.FirstName} {user.LastName}"
            });
        }
    }

    public class RegisterCustomerRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }             // Restaurant Name
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string RestaurantPhone { get; set; }
        public string RestaurantEmail { get; set; }
        public string Website { get; set; }
    }
}
