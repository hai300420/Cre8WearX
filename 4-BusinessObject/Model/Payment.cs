using System;
using System.Collections.Generic;

namespace BusinessObject.Model;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? DepositPaid { get; set; }

    public decimal? DepositAmount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Order Order { get; set; } = null!;
}
