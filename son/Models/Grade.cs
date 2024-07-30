using System;
using System.Collections.Generic;

namespace son.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int EnrollmentId { get; set; }

    public decimal? GradeStudent { get; set; }

    public int CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Enrollment Enrollment { get; set; } = null!;
}
