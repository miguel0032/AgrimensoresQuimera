namespace ParcelaConsultingWeb.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Rol { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}
