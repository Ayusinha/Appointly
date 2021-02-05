using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.Models
{
    public class ApplicationUser : DbContext
    {
        public ApplicationUser(DbContextOptions<ApplicationUser> options) : base(options)
        {

        }
        public DbSet<Users> Users { get; set; }
    }
}
