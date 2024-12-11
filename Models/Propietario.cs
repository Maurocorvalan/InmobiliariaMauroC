namespace Inmobiliaria.Models;
using System.ComponentModel.DataAnnotations;

public class Propietario
{
    public int IdPropietario { get; set; }

    [Required(ErrorMessage = "El Nombre es obligatorio")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El Nombre solo debe contener letras y espacios")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "El Apellido es obligatorio")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El Apellido solo debe contener letras y espacios")]
    public string? Apellido { get; set; }

    [Required(ErrorMessage = "El DNI es obligatorio")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "El DNI solo puede contener letras y números")]
    public string? Dni { get; set; }

    [Required(ErrorMessage = "El Teléfono es obligatorio")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "El Correo Electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
    public string? Email { get; set; }

    public override string ToString()
    {
        return $"{Nombre} {Apellido} {Dni}";
    }
}
