using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class Restaurant
{
    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? LogoUrl { get; set; }

    public string? CoverImageUrl { get; set; }

    public int OwnerUserId { get; set; }

    public int? PlanId { get; set; }

    public bool? IsActive { get; set; }

    public string? Language { get; set; }

    public string? Currency { get; set; }

    public decimal? TaxRate { get; set; }

    public decimal? ServiceChargeRate { get; set; }

    public string? OrderNumberPrefix { get; set; }

    public bool? AutoAcceptOrders { get; set; }

    public int? EstimatedPrepTime { get; set; }

    public bool? AllowSpecialInstructions { get; set; }

    public bool? RequireCustomerInfo { get; set; }

    public bool? DisplayPricesWithTax { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User OwnerUser { get; set; } = null!;

    public virtual ICollection<RestaurantStaff> RestaurantStaffs { get; set; } = new List<RestaurantStaff>();

    public virtual ICollection<RestaurantTable> RestaurantTables { get; set; } = new List<RestaurantTable>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
}
