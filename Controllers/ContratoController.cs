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

    // Verificar si hay un mensaje de éxito en TempData y pasarlo a la vista
    if (TempData["SuccessMessage"] != null)
    {
        ViewData["SuccessMessage"] = TempData["SuccessMessage"];
    }

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
    try
    {
        if (contrato.IdContrato > 0)
        {
            rc.ModificarContrato(contrato);
            TempData["SuccessMessage"] = "Contrato actualizado correctamente.";
        }
        else
        {
            rc.CrearContrato(contrato);
            TempData["SuccessMessage"] = "Contrato creado correctamente.";
        }
        return RedirectToAction(nameof(Index));
    }
    catch (Exception)
    {
        TempData["ErrorMessage"] = "Fechas no disponibles para este Inmueble";
        return RedirectToAction(nameof(Crear));
    }
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


public IActionResult Eliminar(int id)
{
    RepositorioContrato rc = new RepositorioContrato();
    try
    {
        rc.EliminarContrato(id);
        TempData["SuccessMessage"] = "Contrato eliminado correctamente.";
    }
    catch (Exception)
    {
        TempData["ErrorMessage"] = "No se pudo eliminar el contrato.";
    }
    return RedirectToAction(nameof(Index));
}


}
