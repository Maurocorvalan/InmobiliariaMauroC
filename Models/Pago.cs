using System;

namespace Inmobiliaria.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public DateTime FechaPago { get; set; } =DateTime.MinValue;
        public decimal Monto { get; set; } = 0;
        public string? Detalle { get; set; } = string.Empty;
        public bool Estado { get; set; } = false;
 

        public int IdContrato { get; set; }
        public Contrato? Contrato { get; set; }

        public override string ToString()
        {
            return $"ID: {IdPago}  /FECHA: {FechaPago} /MONTO: {Monto}";
        }
    }
}
