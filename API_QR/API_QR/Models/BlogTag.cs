using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class BlogTag
{
    public int TagId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
