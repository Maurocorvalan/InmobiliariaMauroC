using System;

namespace Inmobiliaria.Models
{
    public class Contrato
    {
        public int IdContrato { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public DateTime? FechaTerminacionEfectiva { get; set; }
        public decimal MontoAlquiler { get; set; }
        public Boolean Estado { get; set; }
        public int MesesAdeudados { get; set; } // Asegúrate de que el tipo coincide con la base de datos
        public int IdInquilino { get; set; }
        public int IdInmueble { get; set; }
        public Inquilino? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }


        public override string ToString()
        {
            return $"Contrato: {IdContrato} Inquilino: {Inquilino} Inmueble: {Inmueble} /MONTO: {MontoAlquiler} ";
        }



        public decimal CalcularMulta(DateTime fechaTerminacion)
        {
            int totalMesesContrato = ((FechaFinalizacion.Year - FechaInicio.Year) * 12) + (FechaFinalizacion.Month - FechaInicio.Month);

            int mesesCumplidos = ((fechaTerminacion.Year - FechaInicio.Year) * 12) + (fechaTerminacion.Month - FechaInicio.Month);

            if (mesesCumplidos < (totalMesesContrato / 2))
            {
                return MontoAlquiler * 2;
            }
            else
            {
                return MontoAlquiler;
            }
        }

    }
}