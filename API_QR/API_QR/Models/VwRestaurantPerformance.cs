using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class VwRestaurantPerformance
{
    public int RestaurantId { get; set; }

    public string RestaurantName { get; set; } = null!;

    public string? City { get; set; }

    public DateTime? JoinedDate { get; set; }

    public int? DaysActive { get; set; }

    public int? TotalOrders { get; set; }

    public int? CompletedOrders { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal AverageOrderValue { get; set; }

    public int? TotalTables { get; set; }

    public int? TotalMenuItems { get; set; }

    public double AverageRating { get; set; }

    public int? TotalReviews { get; set; }
}
