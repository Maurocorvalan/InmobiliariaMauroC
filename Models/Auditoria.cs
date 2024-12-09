using System;

namespace Inmobiliaria.Models
{
    public class Auditoria
    {
        public int Id { get; set; }
        public string Accion { get; set; } = string.Empty; 
        public string Usuario { get; set; } = string.Empty; 
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? Detalle { get; set; } 

        public override string ToString()
        {
            return $"Accion: {Accion}, Usuario: {Usuario}, Fecha: {Fecha}, Detalle: {Detalle}";
        }
    }
}
