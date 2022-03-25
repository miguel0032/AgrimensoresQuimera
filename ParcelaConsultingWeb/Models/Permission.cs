using System.Collections.Generic;

namespace ParcelaConsultingWeb.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Entity { get; set; }


        public virtual ICollection<PermissionByRole> Permissions { get; set; }
    }
}
