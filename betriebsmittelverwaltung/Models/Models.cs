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
        Maschine,
        Werkzeug,
        Ressource
    }

    public enum HistoryType
    {
        CheckOut,
        CheckIn
    }

    public enum OrderStatus
    {
        Aktiv,
        Erledigt
    }

    public enum ReturnStatus
    {
        unbestätigt,
        bestätigt
        
    }

    public enum MaintenanceInterval
    {
        Jährlich,
        Zweijährlich,
        Dreijährlich
    }

    public class ConstructionSite
    {
        public int Id { get; set; }
        [MinLength(2, ErrorMessage = "Der Name muss mindestens 2 Zeichen umfassen.")]
        public string Name { get; set; }
        [MinLength(5, ErrorMessage = "Die Beschreibung muss mindestens 5 Zeichen umfassen.")]
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }
        
        [Display(Name = "Bauleiter")]
        public User Manager { get; set; }
        [Display(Name = "Ressourcen")]
        public ICollection<Resource> Resources { get; set; }
        

    }
    

    public class Order
    {
        public int Id { get; set; }
        [Display(Name = "Check Out")]
        public DateTime CheckOut { get; set; }
        [Display(Name = "Auftragsstatus")]
        public OrderStatus OrderStatus { get; set; }
        [Display(Name = "Ressource")]
        public Resource Resource { get; set; }
        [Display(Name = "Baustelle")]
        public ConstructionSite ConstructionSite { get; set; }
        [Display(Name = "Ersteller")]
        public User Creator { get; set; }
    }

    public class Return
    {
        public int Id { get; set; }
        [Display(Name = "Check In")]
        public DateTime CheckIn { get; set; }
        [Display(Name = "Retourenstatus")]
        public ReturnStatus ReturnStatus { get; set; }
        [Display(Name = "Ressource")]
        public Resource Resource { get; set; }
        [Display(Name = "Ersteller")]
        public User Creator { get; set; }
    }

    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Kaufdatum")]
        public DateTime BuyDate { get; set; }
        [Display(Name = "Typ")]
        public ResourceType Type { get; set; }
        [Display(Name = "Wartungsintervall")]
        public MaintenanceInterval MaintenanceInterval { get; set; }
        [Display(Name = "Verfügbar")]
        public bool Available { get; set; }
        [Display(Name = "Nutzungsrate")]
        public double UtilizationRate { get; set; }
        [Display(Name = "Baustelle")]
        public ConstructionSite ConstructionSite { get; set; }
        [Display(Name = "Ressourcenhistorien")]
        public ICollection<ResourceHistory> ResourceHistories { get; set; }
        [Display(Name = "Retouren")]
        public ICollection<Return> Returns { get; set; }
        [Display(Name = "Aufträge")]
        public ICollection<Order> Orders { get; set; }
    }

    public class ResourceHistory
    {
        public int Id { get; set; }
        [Display(Name = "Zeitstempel")]
        public TimeSpan TimeStamp { get; set; }
        [Display(Name = "Historientyp")]
        public HistoryType HiType { get; set; }
        [Display(Name = "Ressource")]
        public Resource Resource { get; set; }
       
    }
}
