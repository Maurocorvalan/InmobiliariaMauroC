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

    
    public IActionResult Editar(int idInquilino)
    {
        if (idInquilino > 0)
        {
            RepositorioInquilino ri = new RepositorioInquilino();
            var inquilino = ri.GetInquilino(idInquilino);
            return View(inquilino);
        }
        else
        {
            return View();
        }
    }
    [HttpPost]

    public IActionResult Guardar(Inquilino inquilino)
    {
        RepositorioInquilino ri = new RepositorioInquilino();
        if (inquilino.IdInquilino > 0)
        {
            ri.ModificarInquilino(inquilino);
        }else{    
        ri.CrearInquilino(inquilino);
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Eliminar(int id)
    {
        RepositorioInquilino ri = new RepositorioInquilino();
        ri.EliminarInquilino(id);
        return RedirectToAction(nameof(Index));
    }

}

