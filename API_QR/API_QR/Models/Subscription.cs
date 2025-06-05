using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class Subscription
{
    public int SubscriptionId { get; set; }

    public int RestaurantId { get; set; }

    public int PlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = null!;

    public bool? AutoRenew { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual Restaurant Restaurant { get; set; } = null!;
}
