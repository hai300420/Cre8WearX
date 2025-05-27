using System;
using System.Collections.Generic;

namespace BusinessObject.Model;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public bool Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? Avatar { get; set; }

    public bool IsDeleted { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<CustomizeProduct> CustomizeProducts { get; set; } = new List<CustomizeProduct>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role Role { get; set; } = null!;
}
