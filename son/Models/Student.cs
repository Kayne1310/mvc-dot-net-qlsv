using System;
using System.Collections.Generic;

namespace son.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public int? RoleId { get; set; }

    public string? MajorName { get; set; }

    public string? Username { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Role? Role { get; set; }
}
