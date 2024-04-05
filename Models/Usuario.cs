namespace Inmobiliaria.Models;

public class Usuario{

    public int IdUsuario {get; set;}
    public string? Nombre {get; set;}
    public string? Apellido{get;set;}
    public string? Email{get;set;}

    public string? Clave{get;set;}

    public string? Avatar{get;set;}
    public IFormFile? AvatarFile{get;set;}
    public int Rol{get;set;}

}