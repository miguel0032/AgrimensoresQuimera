using System;
using System.Collections.Generic;

namespace ParcelaConsultingWeb.Models
{
    public class Solicitud
    {
        public static string parcelas { get; internal set; }
        public int Id { get; set; }
        public int Cantidad_parc { get; set; }
        public int MunicipioId { get; set; }
        public string Recibidos { get; set; }
        public string Parcelas { get; set; }
        public string Solar { get; set; }
        public string Manzana { get; set; }
        public string DistritoCatastral { get; set; }
        public string Beneficiario { get; set; }
        public DateTime FechadeEntrada { get; set; }
        public string UsuarioId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string DigitadorInforme { get; set; }
        public int DepartamentoId { get; set; }
        public int NumeroDiarena { get; set; }
        public DateTime FechaEnviado { get; set; }
        public string Comentario { get; set; }
        public int Status { get; set; }

        public Departamento Departamento { get; set; }
        public User Usuario { get; set; }
        public Municipio Municipio { get; set; }
        public IEnumerable<Accion> Acciones { get; set; }

       
    }

    public enum Status
    {
        creado = 0,
        asignado = 1,
        enviado = 2,
        finalizado = 3
    }

}



