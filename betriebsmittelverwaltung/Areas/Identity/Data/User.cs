using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace betriebsmittelverwaltung.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the User class

    public class User : IdentityUser
    {
        public enum UserType
        {
            Admin,
            Bauleiter,
            Lagerist
        }

        [Display(Name ="Vorname")]
        public string ForeName { get; set; }
        [Display(Name = "Nachname")]
        public string LastName { get; set; }
    }
}
