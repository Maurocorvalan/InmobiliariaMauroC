using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers;

public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;

    public PagoController(ILogger<PagoController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        RepositorioPago rp = new RepositorioPago();
        var lista = rp.GetPagos();
        if (TempData["SuccessMessage"] != null)
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
        }
        return View(lista);
    }



    public IActionResult Crear(int idPago)
    {
        RepositorioContrato rc = new RepositorioContrato();
        ViewBag.Contratos = rc.GetContratos();
        if (idPago > 0)
        {
            RepositorioPago rp = new RepositorioPago();
            var pago = rp.GetPago(idPago);
            return View(pago);
        }
        else
        {
            return View();
        }
    }
    public IActionResult Guardar(Pago pago)
    {
        RepositorioPago rp = new RepositorioPago();
        try
        {
            if (pago.IdPago > 0)
            {
                rp.ModificarPago(pago);
                TempData["SuccessMessage"] = "Pago realizado correctamente";
            }
            else
            {
                rp.CrearPago(pago);
                TempData["SuccessMessage"] = "Contrato creado correctamente.";

            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            TempData["ErrorMessage"] = "Fechas no disponibles para este Inmueble";
            return RedirectToAction(nameof(Index));
        }
    }
}