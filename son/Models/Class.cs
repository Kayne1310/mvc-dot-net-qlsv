﻿using System;
using System.Collections.Generic;

namespace son.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public int CourseId { get; set; }

    public int? TeacherId { get; set; }

    public string? Semester { get; set; }

    public int Year { get; set; }

    public string? RoomName { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Teacher? Teacher { get; set; }
}
