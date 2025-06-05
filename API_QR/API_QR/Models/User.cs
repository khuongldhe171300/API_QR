using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public int RoleId { get; set; }

    public bool? IsActive { get; set; }

    public bool? EmailVerified { get; set; }

    public string? VerificationToken { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordExpires { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();

    public virtual ICollection<RestaurantStaff> RestaurantStaffs { get; set; } = new List<RestaurantStaff>();

    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<SupportTicket> SupportTicketAssignedToNavigations { get; set; } = new List<SupportTicket>();

    public virtual ICollection<SupportTicket> SupportTicketUsers { get; set; } = new List<SupportTicket>();
}
