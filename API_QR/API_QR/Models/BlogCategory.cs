﻿using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class BlogCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }
}
