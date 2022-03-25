using System.Collections.Generic;

namespace ParcelaConsultingWeb.Models
{
    public class TipoAccion : BaseModel
    {
        public string Name { get; set; }
        public IEnumerable<Accion> Acciones { get; set; }
    }
}