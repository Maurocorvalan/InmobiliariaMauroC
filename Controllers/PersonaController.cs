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


    [HttpGet]

    public IActionResult Editar(int id)
    {
     if (id>0){
        RepositorioPersona rp = new RepositorioPersona();
        var persona = rp.GetPersona(id);
        return View(persona);
     }else{
        return View();   
     }
    }


	[HttpPost]
    public IActionResult Guardar(Persona persona){
        RepositorioPersona rp = new RepositorioPersona();   
        if(persona.Id>0){
            rp.ModificarPersona(persona);
        }
        rp.AltaPersona(persona);
        return RedirectToAction(nameof(Index));
    }

	public IActionResult Eliminar(int id)
	{
		RepositorioPersona rp = new RepositorioPersona();
		rp.EliminaPersona(id);
		return RedirectToAction(nameof(Index));
	}
}
