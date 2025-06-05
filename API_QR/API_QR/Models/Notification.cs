using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public int? RestaurantId { get; set; }

    public int? TicketId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Type { get; set; }

    public bool? IsRead { get; set; }

    public bool? IsTicketResponse { get; set; }

    public string? RedirectUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Restaurant? Restaurant { get; set; }

    public virtual SupportTicket? Ticket { get; set; }

    public virtual User? User { get; set; }
}
