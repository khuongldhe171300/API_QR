using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class VwActiveRestaurant
{
    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? LogoUrl { get; set; }

    public string? OwnerName { get; set; }

    public string OwnerEmail { get; set; } = null!;

    public string? OwnerPhone { get; set; }

    public string? PlanName { get; set; }

    public decimal? PlanPrice { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? SubscriptionStatus { get; set; }

    public int IsSubscriptionActive { get; set; }

    public int? DaysUntilExpiry { get; set; }
}
