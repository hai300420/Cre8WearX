using System;
using System.Collections.Generic;

namespace BusinessObject.Model;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsRead { get; set; }

    public virtual User User { get; set; } = null!;
}
