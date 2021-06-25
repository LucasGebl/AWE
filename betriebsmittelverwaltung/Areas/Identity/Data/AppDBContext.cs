using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWE_Projekt.Models;
using betriebsmittelverwaltung.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace betriebsmittelverwaltung.Data
{
    public class AppDBContext : IdentityDbContext<User>
    {
        public DbSet<User> AppUsers { get; set; }
        public DbSet<ConstructionSite> ConstructionSites { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<Resource> Resources { get; set; }


        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=database_bmv;Integrated Security=True;Pooling=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
