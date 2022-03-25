namespace ParcelaConsultingWeb.Models
{
    public class Accion : BaseModel
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int TipoAccionId { get; set; }
        public int SolicitudId { get; set; }

        public TipoAccion TipoAccion { get; set; }
        public Solicitud Solicitud { get; set; }
    }
}
