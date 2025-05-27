using BusinessObject.Enum;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Model;

public partial class OrderStage
{
    public int OrderStageId { get; set; }

    public int OrderId { get; set; }

    public string OrderStageName { get; set; } = null!;



    public DateTime? UpdatedDate { get; set; }

    [JsonIgnore]
    public virtual Order Order { get; set; } = null!;


}
