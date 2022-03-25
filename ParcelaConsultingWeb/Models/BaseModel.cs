using System;

namespace ParcelaConsultingWeb.Models
{
    public class BaseModel
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime Create { get; set; }
        public DateTime LastModifier { get; set; }
    }
}
