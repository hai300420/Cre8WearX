using System;
using System.Collections.Generic;

namespace BusinessObject.Model;

public partial class CustomizeProduct
{
    public int CustomizeProductId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string? ShirtColor { get; set; }

    public string? FullImage { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<DesignElement> DesignElements { get; set; } = new List<DesignElement>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
