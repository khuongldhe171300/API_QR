using API_QR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_QR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {

        private readonly SmartQrdineOptimizedContext _context;
        public BlogController(SmartQrdineOptimizedContext context)
        {
            _context = context;
        }

       
        // GET: api/BlogPosts
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var posts = _context.BlogPosts
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return Ok(posts);
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var post = _context.BlogPosts.Find(id);
            if (post == null)
                return NotFound(new { message = "Không tìm thấy bài viết." });

            return Ok(post);
        }




        [HttpPost("create")]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Không thể xác định người dùng.");
            }

            try
            {
                if (!await HasRestaurantAccess(userId)) 
                {
                    return Forbid();
                }

                var post = new BlogPost
                {
                    Title = request.Title,
                    Slug = request.Slug ?? GenerateSlug(request.Title),
                    Content = request.Content,
                    FeaturedImageUrl = request.FeaturedImageURL,
                    Excerpt = request.Excerpt,
                    AuthorId = userId,  
                    IsPublished = request.IsPublished ?? false,
                    ViewCount = 0,
                    PublishedAt = request.PublishedAt,
                    CategoryIds = request.CategoryIDs,
                    TagIds = request.TagIDs,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.BlogPosts.Add(post);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = post.PostId },
                    new { message = "Tạo bài viết thành công", postId = post.PostId });
            }
            catch (Exception ex)
            {
                var baseException = ex.GetBaseException();
                return StatusCode(500, new
                {
                    message = "Lỗi server",
                    error = ex.Message,
                    inner = baseException?.Message
                });
            }
        }


        // PUT: api/BlogPosts/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BlogPost updatedPost)
        {
            if (updatedPost == null || id != updatedPost.PostId)
                return BadRequest(new { message = "ID không hợp lệ." });

            var existingPost = _context.BlogPosts.Find(id);
            if (existingPost == null)
                return NotFound(new { message = "Không tìm thấy bài viết để cập nhật." });

            // Cập nhật dữ liệu
            existingPost.Title = updatedPost.Title;
            existingPost.Slug = updatedPost.Slug;
            existingPost.Content = updatedPost.Content;
            existingPost.FeaturedImageUrl = updatedPost.FeaturedImageUrl;
            existingPost.Excerpt = updatedPost.Excerpt;
            existingPost.AuthorId = 1;
            existingPost.IsPublished = updatedPost.IsPublished;
            existingPost.ViewCount = updatedPost.ViewCount;
            existingPost.PublishedAt = updatedPost.PublishedAt;
            existingPost.CategoryIds = updatedPost.CategoryIds;
            existingPost.TagIds = updatedPost.TagIds;
            existingPost.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok(existingPost);
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var post = _context.BlogPosts.Find(id);
            if (post == null)
                return NotFound(new { message = "Không tìm thấy bài viết để xoá." });

            _context.BlogPosts.Remove(post);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/Blog/top-viewed
        [HttpGet("top-viewed")]
        public IActionResult GetTopViewedPost()
        {
            var post = _context.BlogPosts
                .Where(p => p.IsPublished == true)
                .OrderByDescending(p => p.ViewCount)
                .Select(p => new
                {
                    p.PostId,
                    p.Title,
                    p.Slug,
                    p.Excerpt,
                    p.FeaturedImageUrl,
                    p.ViewCount,
                    p.PublishedAt,
                    p.CategoryIds,
                    p.TagIds
                })
                .FirstOrDefault();

            if (post == null)
            {
                return NotFound(new { message = "Không có bài viết nào." });
            }

            return Ok(post);
        }

        private async Task<bool> HasRestaurantAccess(int restaurantId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Admin")
                return true;

           

            return await _context.RestaurantStaffs
                .AnyAsync(s => s.RestaurantId == restaurantId && s.UserId == currentUserId);
        }

        public class CreateBlogPostRequestDto
        {
            public string Title { get; set; }
            public string Slug { get; set; } 
            public string Content { get; set; }
            public string FeaturedImageURL { get; set; }
            public string Excerpt { get; set; }
            public int AuthorID { get; set; }
            public bool? IsPublished { get; set; }
            public DateTime? PublishedAt { get; set; }
            public string CategoryIDs { get; set; } 
            public string TagIDs { get; set; }      
        }

        private string GenerateSlug(string title)
        {
            return title.ToLower()
                .Trim()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("?", "")
                .Replace(":", "")
                .Replace(";", "")
                .Replace("!", "")
                .Replace("’", "")
                .Replace("\"", "")
                .Replace("–", "-");
        }


        // GET: api/Blog/categories
        [HttpGet("categories-blog")]
        public IActionResult GetCategories()
        {
            var categories = _context.BlogCategories
                .OrderBy(c => c.Name)
                .Select(c => new { c.CategoryId, c.Name })
                .ToList();

            return Ok(categories);
        }

        // GET: api/Blog/tags
        [HttpGet("tags-blog")]
        public IActionResult GetTags()
        {
            var tags = _context.BlogTags
                .OrderBy(t => t.Name)
                .Select(t => new { t.TagId, t.Name })
                .ToList();

            return Ok(tags);
        }


    }
}
