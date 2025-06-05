using System;
using System.Collections.Generic;

namespace API_QR.Models;

public partial class MenuItemImage
{
    public int ImageId { get; set; }

    public int ItemId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int? DisplayOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual MenuItem Item { get; set; } = null!;
}
