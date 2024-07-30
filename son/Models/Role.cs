using System;
using System.Collections.Generic;

namespace son.Models;

public partial class Role
{
    public int RoleId { get; set; }

    // test
    public string? RoleName { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
