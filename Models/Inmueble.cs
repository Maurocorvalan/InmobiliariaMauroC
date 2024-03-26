 namespace Inmobiliaria.Models;

public class Inmueble
{
    public int IdInmueble { get; set; }
    public string? Direccion  { get; set; }
    public int Ambientes  { get; set; }

    public int Superficie  { get; set; }

    public decimal Latitud { get; set;}

    public decimal Longitud { get; set; }
    
    public int IdPropietario{get; set;}
    public Propietario? Propietario { get; set; }
}
