using System;

namespace ParcelaConsultingWeb.ViewModels
{
    public class ConcesionesListViewModel
    {

        public int Id { get; internal set; }
        public string NoExpediente { get; set; }
        public string Remitente { get; set; }
        public string Asunto { get; set; }
        public string Asignado { get; set; }
        public string DiarenaNo { get; set; }
        public string FechaAsignacion { get; set; }
        public string FechaSalida { get; set; }
        public int Status { get; set; }
        public string FechaEnviado { get; set; }
        public string AsignarUsuario { get; set; }
        public string Usuario { get; set; }
        public string Departamento { get; set; }



    }
}
