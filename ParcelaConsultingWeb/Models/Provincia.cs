using System.Collections.Generic;

namespace ParcelaConsultingWeb.Models
{
    public class Provincia
    {
        public int ProvinciaId { get; set; }
        public string Name { get; set; }

        public IEnumerable<Municipio> Municipios { get; set; }
    }
}
