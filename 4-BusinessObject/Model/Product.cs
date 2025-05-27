using System;
using System.Collections.Generic;

namespace BusinessObject.Model;

public partial class Product
{
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public string? ProductName { get; set; }

    public decimal? Price { get; set; }

    public int StockInStorage { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<CustomizeProduct> CustomizeProducts { get; set; } = new List<CustomizeProduct>();

    public virtual ICollection<DesignArea> DesignAreas { get; set; } = new List<DesignArea>();
}
