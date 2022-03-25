namespace ParcelaConsultingWeb.ViewModels
{
    public class EntitiesByRole
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
