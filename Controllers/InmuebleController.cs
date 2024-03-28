using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers;

public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;

    public InmuebleController(ILogger<InmuebleController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        RepositorioInmueble rinm = new RepositorioInmueble();
        var lista = rinm.GetInmuebles();
        return View(lista);
    }
        [HttpGet]


    public IActionResult Crear(int idInmueble)
    {
        RepositorioPropietarios rpro = new RepositorioPropietarios();
        ViewBag.Propietarios = rpro.GetPropietarios();
        if (idInmueble > 0)
        {
            RepositorioInmueble rinm = new RepositorioInmueble();
            var inmueble = rinm.GetInmueble(idInmueble);
            return View(inmueble);
        }
        else
        {
            return View();
        }
    }
    [HttpGet]


    public IActionResult Editar(int idInmueble)
    {
        RepositorioPropietarios rpro = new RepositorioPropietarios();
        ViewBag.Propietarios = rpro.GetPropietarios();
        if (idInmueble > 0)
        {
            RepositorioInmueble rinm = new RepositorioInmueble();
            var inmueble = rinm.GetInmueble(idInmueble);
            return View(inmueble);
        }
        else
        {
            return View();
        }
    }
    [HttpPost]

    public IActionResult Guardar(Inmueble inmueble)
    {
        RepositorioInmueble rinm = new RepositorioInmueble();
        if (inmueble.IdInmueble > 0)
        {
            rinm.ModificarInmueble(inmueble);
        }
        else
        {
            rinm.CrearInmueble(inmueble);
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Eliminar(int id)
    {
        RepositorioInmueble rinm = new RepositorioInmueble();
        rinm.EliminarInmueble(id);
        return RedirectToAction(nameof(Index));
    }



}

