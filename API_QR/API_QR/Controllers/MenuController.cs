using API_QR.DTOs;
using API_QR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_QR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly SmartQrdineOptimizedContext _context;

        public MenuController(SmartQrdineOptimizedContext context)
        {
            _context = context;
        }

        // Menu Categories
        // lấy ra các danh mục món ăn của một nhà hàng
        [HttpGet("categories")]
        public async Task<IActionResult> GetMenuCategories()
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

                var categories = await _context.MenuCategories
                    .Where(c => c.RestaurantId == restaurants.RestaurantId && c.IsActive == true)
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Name)
                    .Select(c => new
                    {
                        c.CategoryId,
                        c.Name,
                        c.Description,
                        c.ImageUrl,
                        c.DisplayOrder,
                        ItemCount = c.MenuItems.Count(i => i.IsAvailable == true)
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }


        // Tạo danh mục món ăn mới
        [HttpPost("categories")]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> CreateMenuCategory([FromBody] CreateMenuCategoryRequestDto request)
        {
            try
            {
                // Check restaurant access
                if (!await HasRestaurantAccess(request.RestaurantID))
                {
                    return Forbid();
                }

                var category = new MenuCategory
                {
                    RestaurantId = request.RestaurantID,
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.ImageURL,
                    DisplayOrder = request.DisplayOrder,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.MenuCategories.Add(category);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMenuCategories), new { restaurantId = request.RestaurantID },
                    new { message = "Tạo danh mục thành công", categoryId = category.CategoryId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }


        // Cập nhật danh mục món ăn (categori)
        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> UpdateMenuCategory(int id, [FromBody] UpdateMenuCategoryRequestDto request)
        {
            try
            {
                var category = await _context.MenuCategories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục" });
                }

                if (!await HasRestaurantAccess(category.RestaurantId))
                {
                    return Forbid();
                }

                category.Name = request.Name ?? category.Name;
                category.Description = request.Description ?? category.Description;
                category.ImageUrl = request.ImageURL ?? category.ImageUrl;
                category.DisplayOrder = request.DisplayOrder ?? category.DisplayOrder;
                category.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật danh mục thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }


        // Xóa danh mục món ăn
        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> DeleteMenuCategory(int id)
        {
            try
            {
                var category = await _context.MenuCategories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục" });
                }

                if (!await HasRestaurantAccess(category.RestaurantId))
                {
                    return Forbid();
                }

                category.IsActive = false;
                category.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa danh mục thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        // Menu Items by nha hang
        [HttpGet("items")]
        public async Task<IActionResult> GetMenuItems()
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

                var query = _context.MenuItems
                    .Include(i => i.Category)
                    .Include(i => i.MenuItemImages)
                    .Where(i => i.RestaurantId == restaurants.RestaurantId);

                var items = await query
                    .OrderBy(i => i.Category.DisplayOrder)
                    .ThenBy(i => i.DisplayOrder)
                    .ThenBy(i => i.Name)
                    .Select(i => new
                    {
                        i.ItemId,
                        i.CategoryId,
                        CategoryName = i.Category.Name,
                        i.Name,
                        i.Description,
                        i.Price,
                        i.DiscountPrice,
                        i.ImageUrl,
                        i.PreparationTime,
                        i.Calories,
                        i.Ingredients,
                        i.AllergenInfo,
                        i.IsVegetarian,
                        i.IsVegan,
                        i.IsGlutenFree,
                        i.IsSpicy,
                        i.IsFeatured,
                        i.IsAvailable,
                        i.Options,
                        i.Addons,
                        i.DisplayOrder,
                        AdditionalImages = i.MenuItemImages.OrderBy(img => img.DisplayOrder).Select(img => img.ImageUrl).ToList(),
                        AverageRating = _context.Reviews
                            .Where(r => r.ItemId == i.ItemId && r.ReviewType == "Item")
                            .Average(r => (double?)r.Rating) ?? 0,
                        ReviewCount = _context.Reviews
                            .Count(r => r.ItemId == i.ItemId && r.ReviewType == "Item")
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }






        // Lấy thông tin chi tiết món ăn
        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            try
            {
                var item = await _context.MenuItems
                    .Include(i => i.Category)
                    .Include(i => i.MenuItemImages)
                    .FirstOrDefaultAsync(i => i.ItemId == id);

                if (item == null)
                {
                    return NotFound(new { message = "Không tìm thấy món ăn" });
                }

                var reviews = await _context.Reviews
                    .Where(r => r.ItemId == id && r.ReviewType == "Item")
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .Select(r => new
                    {
                        r.ReviewId,
                        r.CustomerName,
                        r.Rating,
                        r.Comment,
                        r.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new
                {
                    item.ItemId,
                    item.CategoryId,
                    CategoryName = item.Category?.Name,
                    item.Name,
                    item.Description,
                    item.Price,
                    item.DiscountPrice,
                    item.ImageUrl,
                    item.PreparationTime,
                    item.Calories,
                    item.Ingredients,
                    item.AllergenInfo,
                    item.IsVegetarian,
                    item.IsVegan,
                    item.IsGlutenFree,
                    item.IsSpicy,
                    item.IsFeatured,
                    item.IsAvailable,
                    item.Options,
                    item.Addons,
                    item.DisplayOrder,
                    AdditionalImages = item.MenuItemImages.OrderBy(img => img.DisplayOrder).Select(img => img.ImageUrl).ToList(),
                    Reviews = reviews,
                    AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
                    ReviewCount = reviews.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }





        [HttpPost("items")]
        [Authorize(Roles = "RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> CreateMenuItem([FromForm] CreateMenuItemRequestDto request)
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

                string optionsJson = string.IsNullOrWhiteSpace(request.Options)
                    ? LoadJsonFromFile($"options_{restaurants.RestaurantId}.json")
                    : request.Options;

                string addonsJson = string.IsNullOrWhiteSpace(request.Addons)
                    ? LoadJsonFromFile($"addons_{restaurants.RestaurantId}.json")
                    : request.Addons;

                string imageUrl = "";
                if (request.ImageFile != null && request.ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(request.ImageFile.FileName);
                    var filePath = Path.Combine("wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.ImageFile.CopyToAsync(stream);
                    }

                    imageUrl = $"/uploads/{fileName}";
                }


                var item = new MenuItem
                {
                    RestaurantId = restaurants.RestaurantId,
                    CategoryId = request.CategoryID,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    DiscountPrice = request.DiscountPrice,
                    ImageUrl = imageUrl,
                    PreparationTime = request.PreparationTime,
                    Calories = request.Calories,
                    Ingredients = request.Ingredients,
                    AllergenInfo = request.AllergenInfo,
                    IsVegetarian = request.IsVegetarian,
                    IsVegan = request.IsVegan,
                    IsGlutenFree = request.IsGlutenFree,
                    IsSpicy = request.IsSpicy,
                    IsFeatured = request.IsFeatured,
                    IsAvailable = request.IsAvailable,
                    DisplayOrder = request.DisplayOrder,
                    Options = optionsJson,
                    Addons = addonsJson,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };


                _context.MenuItems.Add(item);
                await _context.SaveChangesAsync();

                if (request.AdditionalImages?.Any() == true)
                {
                    var images = request.AdditionalImages.Select((url, index) => new MenuItemImage
                    {
                        ItemId = item.ItemId,
                        ImageUrl = url,
                        DisplayOrder = index,
                        CreatedAt = DateTime.Now
                    }).ToList();

                    _context.MenuItemImages.AddRange(images);
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(GetMenuItem), new { id = item.ItemId },
                    new { message = "Tạo món ăn thành công", itemId = item.ItemId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        private string LoadJsonFromFile(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", fileName);
            return System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : "[]";
        }


        // Cập nhật món ăn
        [HttpPut("items/{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemRequestDto request)
        {
            try
            {
                var item = await _context.MenuItems.FindAsync(id);
                if (item == null)
                    return NotFound(new { message = "Không tìm thấy món ăn" });

                if (!await HasRestaurantAccess(item.RestaurantId))
                    return Forbid();

                // === Load Options/Addons nếu không có trong request ===
                var optionsJson = string.IsNullOrWhiteSpace(request.Options)
                    ? LoadJsonFromFile($"options_{item.RestaurantId}.json")
                    : request.Options;

                var addonsJson = string.IsNullOrWhiteSpace(request.Addons)
                    ? LoadJsonFromFile($"addons_{item.RestaurantId}.json")
                    : request.Addons;

                // === Cập nhật ===
                item.CategoryId = request.CategoryID ?? item.CategoryId;
                item.Name = request.Name ?? item.Name;
                item.Description = request.Description ?? item.Description;
                item.Price = request.Price ?? item.Price;
                item.DiscountPrice = request.DiscountPrice ?? item.DiscountPrice;
                item.ImageUrl = request.ImageURL ?? item.ImageUrl;
                item.PreparationTime = request.PreparationTime ?? item.PreparationTime;
                item.Calories = request.Calories ?? item.Calories;
                item.Ingredients = request.Ingredients ?? item.Ingredients;
                item.AllergenInfo = request.AllergenInfo ?? item.AllergenInfo;
                item.IsVegetarian = request.IsVegetarian ?? item.IsVegetarian;
                item.IsVegan = request.IsVegan ?? item.IsVegan;
                item.IsGlutenFree = request.IsGlutenFree ?? item.IsGlutenFree;
                item.IsSpicy = request.IsSpicy ?? item.IsSpicy;
                item.IsFeatured = request.IsFeatured ?? item.IsFeatured;
                item.IsAvailable = request.IsAvailable ?? item.IsAvailable;
                item.DisplayOrder = request.DisplayOrder ?? item.DisplayOrder;
                item.Options = optionsJson;
                item.Addons = addonsJson;
                item.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật món ăn thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }



        // Xóa món ăn (đánh dấu là không khả dụng)
        [HttpDelete("items/{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner,RestaurantStaff")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            try
            {
                var item = await _context.MenuItems.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = "Không tìm thấy món ăn" });
                }

                if (!await HasRestaurantAccess(item.RestaurantId))
                {
                    return Forbid();
                }

                item.IsAvailable = false;
                item.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa món ăn thành công" });
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


        //tìm kiếm món ăn 
        [HttpGet("search")]
        public async Task<IActionResult> SearchMenuItems([FromQuery] int restaurantId, [FromQuery] string query)
        {
            try
            {
                var items = await _context.MenuItems
                    .Include(i => i.Category)
                    .Where(i => i.RestaurantId == restaurantId && i.IsAvailable == true &&
                                (i.Name.Contains(query)))
                    .OrderBy(i => i.Category.DisplayOrder)
                    .ThenBy(i => i.DisplayOrder)
                    .ThenBy(i => i.Name)
                    .Select(i => new
                    {
                        i.ItemId,
                        i.CategoryId,
                        CategoryName = i.Category.Name,
                        i.Name,
                        i.Description,
                        i.Price,
                        i.DiscountPrice,
                        i.ImageUrl,
                        i.PreparationTime,
                        i.Calories,
                        i.Ingredients,
                        i.AllergenInfo,
                        i.IsVegetarian,
                        i.IsVegan,
                        i.IsGlutenFree,
                        i.IsSpicy,
                        i.IsFeatured,
                        i.IsAvailable,
                        AdditionalImages = i.MenuItemImages.OrderBy(img => img.DisplayOrder).Select(img => img.ImageUrl).ToList(),
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        //---------------------------------
        // GET: api/restaurants/3/menu
        [HttpGet("{restaurantId}/menu")]
        [AllowAnonymous ]
        public async Task<IActionResult> GetMenu(int restaurantId)
        {
            var menu = await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId && m.IsAvailable == true)
                .Include(m => m.Category)
                .Select(m => new {
                    m.ItemId,
                    m.Name,
                    m.Price,
                    m.Description,
                    CategoryName = m.Category.Name,
                    m.ImageUrl
                }).ToListAsync();

            return Ok(menu);
        }

        // GET: api/restaurants/3/tables/A06
        [HttpGet("{restaurantId}/tables/{tableNumber}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTable(int restaurantId, string tableNumber)
        {
            var table = await _context.RestaurantTables
                .FirstOrDefaultAsync(t => t.RestaurantId == restaurantId && t.TableNumber == tableNumber);

            if (table == null)
                return NotFound(new { message = "Không tìm thấy bàn." });

            return Ok(table);
        }

    }
   

   
}
