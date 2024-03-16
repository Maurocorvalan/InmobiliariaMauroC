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

}
