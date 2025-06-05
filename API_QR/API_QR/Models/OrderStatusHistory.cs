﻿using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class OrderStatusHistory
{
    public int HistoryId { get; set; }

    public int OrderId { get; set; }

    public string? OldStatus { get; set; }

    public string NewStatus { get; set; } = null!;

    public int? ChangedBy { get; set; }

    public DateTime? ChangedAt { get; set; }

    public virtual User? ChangedByNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;
}
