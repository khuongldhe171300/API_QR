using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class MenuItem
{
    public int ItemId { get; set; }

    public int RestaurantId { get; set; }

    public int? CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    public string? ImageUrl { get; set; }

    public int? PreparationTime { get; set; }

    public int? Calories { get; set; }

    public string? Ingredients { get; set; }

    public string? AllergenInfo { get; set; }

    public bool? IsVegetarian { get; set; }

    public bool? IsVegan { get; set; }

    public bool? IsGlutenFree { get; set; }

    public bool? IsSpicy { get; set; }

    public bool? IsFeatured { get; set; }

    public bool? IsAvailable { get; set; }

    public int? DisplayOrder { get; set; }

    public string? Options { get; set; }

    public string? Addons { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual MenuCategory? Category { get; set; }

    public virtual ICollection<MenuItemImage> MenuItemImages { get; set; } = new List<MenuItemImage>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Restaurant Restaurant { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
