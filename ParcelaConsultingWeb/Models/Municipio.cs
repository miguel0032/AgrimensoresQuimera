using System.Collections.Generic;

namespace ParcelaConsultingWeb.Models
{
    public class Municipio
    {
        public int MunicipioId { get; set; }
        public string Name { get; set; }
        public int ProvinciaId { get; set; }

        public IEnumerable<Solicitud> Solicitudes { get; set; }
        public Provincia Provincia { get; set; }
    }
}
