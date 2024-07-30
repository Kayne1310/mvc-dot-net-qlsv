using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using son.Models;

namespace son.Data
{
    public class sonContext : DbContext
    {
        public sonContext (DbContextOptions<sonContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Class> Classes { get; set; }

        public virtual DbSet<Course> Courses { get; set; }

  

        public virtual DbSet<Grade> Grades { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<Teacher> Teachers { get; set; }


       
      
    }
}
