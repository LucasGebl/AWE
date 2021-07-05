using betriebsmittelverwaltung.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public enum HistoryType
    {
        CheckOut,
        CheckIn
    }

    public enum OrderStatus
    {
        active,
        completed
    }

    public enum ReturnStatus
    {
        confirmed,
        uncomfirmed
    }

    public enum MaintenanceInterval
    {
        Yearly,
        EveryTwoYears,
        EveryThreeYears
    }

    public class ConstructionSite
    {
        public int Id { get; set; }
        [MinLength(2, ErrorMessage = "Der Name muss mindestens 2 Zeichen umfassen.")]
        public string Name { get; set; }
        [MinLength(5, ErrorMessage = "Die Beschreibung muss mindestens 5 Zeichen umfassen.")]
        public string Description { get; set; }
        public User Manager { get; set; }
        public ICollection<Resource> Resources { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime CheckOut { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Resource Resource { get; set; }
        public ConstructionSite ConstructionSite { get; set; }
        public User Creator { get; set; }
    }

    public class Return
    {
        public int Id { get; set; }
        public DateTime CheckIn { get; set; }
        public ReturnStatus ReturnStatus { get; set; }
        public Resource Resource { get; set; }
        public User Creator { get; set; }
    }

    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BuyDate { get; set; }
        public ResourceType Type { get; set; }
        public MaintenanceInterval MaintenanceInterval { get; set; }
        public bool Available { get; set; }
        public double UtilizationRate { get; set; }
        public ConstructionSite ConstructionSite { get; set; }
        public ICollection<ResourceHistory> ResourceHistories { get; set; }
        public ICollection<Return> Returns { get; set; }
        public ICollection<Order> Orders { get; set; }
    }

    public class ResourceHistory
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public HistoryType HiType { get; set; }
        public Resource Resource { get; set; }
       
    }
}
