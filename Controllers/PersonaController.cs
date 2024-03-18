using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers;

public class PersonaController : Controller
{
    private readonly ILogger<PersonaController> _logger;

    public PersonaController(ILogger<PersonaController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        RepositorioPersona rp = new RepositorioPersona();
        var lista = rp.GetPersonas();
        return View(lista);
    }

    public IActionResult Editar(int id)
    {
     if (id>0){
        RepositorioPersona rp = new RepositorioPersona();
        var persona = rp.GetPersona(id);
        return View(persona);
     }
     return View();   
    }
    public IActionResult Guardar(Persona persona){
        RepositorioPersona rp = new RepositorioPersona();   
        rp.GuardarPersona(persona);
        return RedirectToAction(nameof(Index));
    }

}
