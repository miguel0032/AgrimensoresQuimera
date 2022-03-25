using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ParcelaConsultingWeb.Models
{
    public class Departamento
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public int Piso { get; set; }

        public IEnumerable<Solicitud> Solicitudes { get; set; }
    }
}
