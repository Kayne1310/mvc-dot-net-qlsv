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

     
    }
}
