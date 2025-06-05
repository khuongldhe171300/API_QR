using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class BlogPost
{
    public int PostId { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? FeaturedImageUrl { get; set; }

    public string? Excerpt { get; set; }

    public int? AuthorId { get; set; }

    public bool? IsPublished { get; set; }

    public int? ViewCount { get; set; }

    public DateTime? PublishedAt { get; set; }

    public string? CategoryIds { get; set; }

    public string? TagIds { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? Author { get; set; }
}
