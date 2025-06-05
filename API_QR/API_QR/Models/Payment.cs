using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string PaymentType { get; set; } = null!;

    public int? OrderId { get; set; }

    public int? SubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public string? Currency { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public string? TransactionId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? BillingInfo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Subscription? Subscription { get; set; }
}
