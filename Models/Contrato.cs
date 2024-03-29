using System;

namespace Inmobiliaria.Models
{
    public class Contrato
    {
        public int IdContrato { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public decimal MontoAlquiler { get; set; }
        public Boolean Estado { get; set; }

        public int IdInquilino { get; set; }
        public int IdInmueble { get; set; }
        public Inquilino? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }
    }
}
