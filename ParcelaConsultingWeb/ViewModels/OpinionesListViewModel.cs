using ParcelaConsultingWeb.Models;

namespace ParcelaConsultingWeb.ViewModels
{
    public class OpinionesListViewModel
    {

        public int Id { get; set; }
        public string Expediente { get; set; }
        public string FechaEntrada { get; set; }
        public string Solicitante { get; set; }
        public string Asunto { get; set; }
        public string Asignados { get; set; }
        public string fechaAsignado { get; set; }
        public string Digitador { get; set; }
        public string Departamento { get; set; }
        public int RemitidoDiarena { get; set; }
        public string FechaRemision { get; set; }
        public string Usuario { get; set; }
        public string AsignarUsuario { get; set; }
        public int Status { get; set; }
    }
}
