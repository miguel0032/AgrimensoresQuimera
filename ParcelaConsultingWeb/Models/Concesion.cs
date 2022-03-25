using System;

namespace ParcelaConsultingWeb.Models
{
    public class Concesion : BaseModel
    {
        public string NoExpediente { get; set; }
        public DateTime Fecha { get; set; }
        public string Remitente { get; set; }
        public string Asunto { get; set; }
        public string Asignado { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public int DepartamentoId { get; set; }
        public string DiarenaNo { get; set; }
        public DateTime FechaEnviado { get; set; }
        public string Comentario { get; set; }
        public int Status { get; set; }
        public string UsuarioId { get; set; }

        public User Usuario { get; set; }

        public Departamento Departamento { get; set; }
    }
}
