 namespace Inmobiliaria.Models;

public class Persona
{
    public int Id { get; set; }
    public string? Nombre  { get; set; }
    public string? Apellido  { get; set; }

    public int Dni  { get; set; }

    public string? Email { get; set;}

}
public enum TipoPersona{
    PersonaNatural =0,
    PersonaJuridica =1
}
