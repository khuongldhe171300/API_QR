using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class MenuCategory
{
    public int CategoryId { get; set; }

    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual Restaurant Restaurant { get; set; } = null!;
}
