using System;
using System.Collections.Generic;

namespace son.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public string? Name { get; set; }

    public string Password { get; set; } = null!;

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int? RoleId { get; set; }

    public string? Username { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual Role? Role { get; set; }
}
