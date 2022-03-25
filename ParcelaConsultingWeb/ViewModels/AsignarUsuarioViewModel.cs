using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ParcelaConsultingWeb.ViewModels
{
    public class AsignarUsuarioViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; internal set; }
        public string ViewName { get; internal set; }
        public ViewDataDictionary ViewData { get; internal set; }
    }
}
