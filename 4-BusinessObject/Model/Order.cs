using System;
using System.Collections.Generic;

namespace BusinessObject.Model;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomizeProductId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string? RecipientName { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? ShippingMethod { get; set; }

    public double? ShippingFee { get; set; }

    public string? Notes { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual CustomizeProduct CustomizeProduct { get; set; } = null!;

    public virtual ICollection<OrderStage> OrderStages { get; set; } = new List<OrderStage>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
