using System;

namespace ParcelaConsultingWeb.Models
{

    public class Opinion : BaseModel
    {
        public string Expediente { get; set; }
        public DateTime FechaEntrada { get; set; }
        public string Solicitante { get; set; }
        public string Asunto { get; set; }
        public string Asignados { get; set; }
        public DateTime fechaAsignado { get; set; }
        public string Digitador { get; set; }
        public int DepartamentoId { get; set; }
        public int RemitidoDiarena { get; set; }
        public DateTime FechaRemision { get; set; }
        public string Comentario { get; set; }

        public string UsuarioId { get; set; }
        public User Usuario { get; set; }

        public Departamento Departamento { get; set; }
        public int Status { get; internal set; }
    }
}
