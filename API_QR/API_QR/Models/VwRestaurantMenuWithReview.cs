using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class VwRestaurantMenuWithReview
{
    public int ItemId { get; set; }

    public int RestaurantId { get; set; }

    public string RestaurantName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    public string? ImageUrl { get; set; }

    public int? PreparationTime { get; set; }

    public int? Calories { get; set; }

    public bool? IsVegetarian { get; set; }

    public bool? IsVegan { get; set; }

    public bool? IsGlutenFree { get; set; }

    public bool? IsSpicy { get; set; }

    public bool? IsFeatured { get; set; }

    public bool? IsAvailable { get; set; }

    public string? Options { get; set; }

    public string? Addons { get; set; }

    public double AverageRating { get; set; }

    public int? ReviewCount { get; set; }
}
