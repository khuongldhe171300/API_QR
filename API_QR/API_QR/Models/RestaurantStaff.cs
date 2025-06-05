using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class RestaurantStaff
{
    public int StaffId { get; set; }

    public int RestaurantId { get; set; }

    public int UserId { get; set; }

    public string? Position { get; set; }

    public bool? IsManager { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Restaurant Restaurant { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
