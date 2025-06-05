using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class RestaurantTable
{
    public int TableId { get; set; }

    public int RestaurantId { get; set; }

    public string TableNumber { get; set; } = null!;

    public int? Capacity { get; set; }

    public string? Location { get; set; }

    public string? Status { get; set; }

    public string? QrcodeUrl { get; set; }

    public string? QrcodeData { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Restaurant Restaurant { get; set; } = null!;
}
