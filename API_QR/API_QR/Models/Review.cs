using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public string ReviewType { get; set; } = null!;

    public int RestaurantId { get; set; }

    public int? ItemId { get; set; }

    public int? OrderId { get; set; }

    public string? CustomerName { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public int? FoodRating { get; set; }

    public int? ServiceRating { get; set; }

    public int? AmbienceRating { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual MenuItem? Item { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Restaurant Restaurant { get; set; } = null!;
}
