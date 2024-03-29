using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Org.BouncyCastle.Asn1.Iana;

namespace Inmobiliaria.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;

    public ContratoController(ILogger<ContratoController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        RepositorioContrato rc = new RepositorioContrato();
        var lista = rc.GetContratos();
        return View(lista);
    }

    public IActionResult Crear(int idContrato)
    {
        RepositorioInquilino ri = new RepositorioInquilino();
        RepositorioInmueble rim = new RepositorioInmueble();
        ViewBag.Inquilinos = ri.GetInquilinos();
        ViewBag.Inmuebles = rim.GetInmuebles();
        if (idContrato > 0)
        {
            RepositorioContrato rc = new RepositorioContrato();
            var contrato = rc.GetContrato(idContrato);
            return View(contrato);
        }
        else
        {
            return View();
        }
    }
    public IActionResult Guardar(Contrato contrato)
    {
        RepositorioContrato rc = new RepositorioContrato();
        if (contrato.IdContrato > 0)
        {
            rc.ModificarContrato(contrato);
        }
        else
        {
            rc.CrearContrato(contrato);
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Editar(int idContrato)
    {
        RepositorioInquilino ri = new RepositorioInquilino();
        RepositorioInmueble rim = new RepositorioInmueble();
        ViewBag.Inquilinos = ri.GetInquilinos();
        ViewBag.Inmuebles = rim.GetInmuebles();
        if (idContrato > 0)
        {
            RepositorioContrato rc = new RepositorioContrato();
            var contrato = rc.GetContrato(idContrato);
            return View(contrato);
        }
        else
        {
            return View();
        }
    }


    public IActionResult Eliminar(int id){
        RepositorioContrato rc = new RepositorioContrato();
        rc.EliminarContrato(id);
        return RedirectToAction(nameof(Index));
    }

}
