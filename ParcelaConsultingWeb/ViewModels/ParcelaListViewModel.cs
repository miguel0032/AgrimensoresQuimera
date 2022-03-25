namespace ParcelaConsultingWeb.ViewModels
{
    public class ParcelaListViewModel
    {
        public int Id { get; set; }
        public string Parcela { get; set; }
        public int Cantidad { get; set; }
        public string Beneficiario { get; set; }
        public string FechaEntrada { get; set; }
        public string Manzana { get; set; }
        public string FechaEnviado { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
    }
}
