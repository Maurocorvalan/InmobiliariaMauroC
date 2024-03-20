using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers;

public class PropietarioController : Controller
{
    private readonly ILogger<PropietarioController> _logger;

    public PropietarioController(ILogger<PropietarioController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        RepositorioPropietarios rp = new RepositorioPropietarios();
        var lista = rp.GetPropietarios();
        return View(lista);
    }
    [HttpGet]

    public IActionResult Editar(int idPropietario)
    {
        if (idPropietario > 0)
        {
            RepositorioPropietarios rp = new RepositorioPropietarios();
            var propietario = rp.GetPropietario(idPropietario);
            return View(propietario);
        }
        else
        {
            return View();
        }
    }
    [HttpPost]

    public IActionResult Guardar(Propietario propietario)
    {
        RepositorioPropietarios rp = new RepositorioPropietarios();
        if (propietario.IdPropietario > 0)
        {
            rp.ModificarPropietario(propietario);
        }else{    
        rp.CrearPropietario(propietario);
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Eliminar(int id)
    {
        RepositorioPropietarios rp = new RepositorioPropietarios();
        rp.EliminarPropietario(id);
        return RedirectToAction(nameof(Index));
    }

}

