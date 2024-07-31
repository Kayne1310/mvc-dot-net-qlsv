using System;
using System.Collections.Generic;

namespace son.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public decimal? GradeStudent { get; set; }

    public int CourseId { get; set; }

    public int? ClassId { get; set; }

    public int? StudentId { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student? Student { get; set; }
}
