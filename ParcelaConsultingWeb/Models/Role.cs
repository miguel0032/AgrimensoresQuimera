using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ParcelaConsultingWeb.Models
{
    public class Role : IdentityRole
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public ICollection<PermissionByRole> Permissions { get; set; }
    }
}
