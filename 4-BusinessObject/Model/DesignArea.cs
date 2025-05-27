using System;
using System.Collections.Generic;

namespace BusinessObject.Model;

public partial class DesignArea
{
    public int DesignAreaId { get; set; }

    public int ProductId { get; set; }

    public string? AreaName { get; set; }

    public string? CoordinateX { get; set; }

    public string? CoordinateY { get; set; }

    public virtual ICollection<DesignElement> DesignElements { get; set; } = new List<DesignElement>();

    public virtual Product Product { get; set; } = null!;
}
