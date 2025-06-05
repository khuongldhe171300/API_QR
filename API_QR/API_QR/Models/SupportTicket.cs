using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class SupportTicket
{
    public int TicketId { get; set; }

    public int? UserId { get; set; }

    public int? RestaurantId { get; set; }

    public string Subject { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Status { get; set; }

    public string? Priority { get; set; }

    public int? AssignedTo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Restaurant? Restaurant { get; set; }

    public virtual User? User { get; set; }
}
