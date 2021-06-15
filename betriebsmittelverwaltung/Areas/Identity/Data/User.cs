using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace betriebsmittelverwaltung.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the User class
    public enum UserType
    {
        Admin,
        Bauleiter,
        Lagerist
    }
    public class User : IdentityUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType Type { get; set; }
    }
}
