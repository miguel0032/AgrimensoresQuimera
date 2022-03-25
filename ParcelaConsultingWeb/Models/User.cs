using Microsoft.AspNetCore.Identity;
using ParcelaConsultingWeb.ViewModels;
using System;
using System.Collections.Generic;

namespace ParcelaConsultingWeb.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public IEnumerable<Solicitud> Solicitudes { get; set; }
        public string FullName { get; set; }

       
    }
}
