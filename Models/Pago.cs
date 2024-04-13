using System;

namespace Inmobiliaria.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string? Detalle { get; set; }
        public bool Estado { get; set; }

        public int IdContrato { get; set; }
        public Contrato? Contrato { get; set; }

        public override string ToString()
        {
            return $"ID: {IdPago}  /FECHA: {FechaPago} /MONTO: {Monto}";
        }
    }
}
