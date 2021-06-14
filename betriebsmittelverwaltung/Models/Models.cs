using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWE_Projekt.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<ConstructionSite> ConstructionSites { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<Resource> Resources { get; set; }
    }
    public enum ResourceType
    {
        Machine,
        Tool,
        Resource
    }

    public enum UserType
    {
        Admin,
        Bauleiter,
        Lagerist
    }
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType Type { get; set; }
    }
    public class ConstructionSite
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public User Manager { get; set; }

    }

    public class Order
    {
        public int Id { get; set; }
        public Resource Resource { get; set; }
        public ConstructionSite ConstructionSite { get; set; }
        public DateTime CheckOut { get; set; }
    }

    public class Return
    {
        public int Id { get; set; }
        public Resource Resource { get; set; }
        public DateTime CheckIn { get; set; }
    }

    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BuyDate { get; set; }
        public ResourceType Type { get; set; }
        public double UtilizationRate { get; set; }
        public ConstructionSite ConstructionSite { get; set; }
    }
}
