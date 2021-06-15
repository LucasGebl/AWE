using betriebsmittelverwaltung.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWE_Projekt.Models
{
   
    public enum ResourceType
    {
        Machine,
        Tool,
        Resource
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
