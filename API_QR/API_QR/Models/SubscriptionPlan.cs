using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class SubscriptionPlan
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string BillingCycle { get; set; } = null!;

    public int? MaxTables { get; set; }

    public int? MaxMenuItems { get; set; }

    public string? Features { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
