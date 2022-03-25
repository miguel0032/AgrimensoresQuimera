using System;

namespace ParcelaConsultingWeb.Models
{
    public class PermissionByRole
    {
        public string Id { get; set; }
        public DateTime LastModified { get; set; }

        public string RoleId { get; set; }
        public Role Role { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }

        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
