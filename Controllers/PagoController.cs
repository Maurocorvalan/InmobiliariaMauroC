using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace Inmobiliaria.Controllers;
[Authorize]

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

        ViewBag.Contratos = rc.GetContratos().Select(c => new
        {
            c.IdContrato,
            Descripcion = $"Contrato {c.IdContrato} - Inquilino: {c.Inquilino.Nombre} {c.Inquilino.Apellido} - Dueño: {c.Inmueble.Duenio.Nombre} {c.Inmueble.Duenio.Apellido} - Dirección: {c.Inmueble.Direccion}",
            c.MontoAlquiler
        }).ToList();

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


    public IActionResult Editar(int idPago)
    {
        try
        {
            RepositorioContrato rc = new RepositorioContrato();
            ViewBag.Contratos = rc.GetContratos();

            if (idPago > 0)
            {
                RepositorioPago rp = new RepositorioPago();
                var pago = rp.GetPago(idPago);

                if (pago == null)
                {
                    TempData["ErrorMessage"] = "El pago solicitado no existe.";
                    return RedirectToAction(nameof(Index));
                }

                return View(pago);
            }
            else
            {
                TempData["ErrorMessage"] = "ID de pago inválido.";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la vista de edición.");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar cargar la edición.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult Guardar(Pago pago)
    {
        RepositorioPago rp = new RepositorioPago();
        RepositorioContrato rc = new RepositorioContrato();

        RepositorioAuditoria ra = new RepositorioAuditoria();
        try
        {
            if (pago.IdPago > 0)
            {
                rp.ModificarPago(pago);
                ra.RegistrarAuditoria("Modificación Pago", User.Identity.Name, $"Pago ID: {pago.IdPago}");
                TempData["SuccessMessage"] = "Pago actualizado correctamente.";
            }
            else
            {
                rp.CrearPago(pago);
                ra.RegistrarAuditoria("Creación Pago", User.Identity.Name, $"Contrato ID: {pago.IdContrato}");
                TempData["SuccessMessage"] = "Pago creado correctamente.";
            }
            // Verificar si el contrato está cancelado y actualizar los meses adeudados
            var contrato = rc.GetContrato(pago.IdContrato);
            if (contrato != null && contrato.FechaTerminacionEfectiva != null && !pago.EsMulta)
            {
                // Recalcular los meses adeudados
                int mesesAdeudados = rp.CalcularPagosAdeudados(
                    contrato.IdContrato,
                    contrato.FechaInicio,
                    contrato.FechaTerminacionEfectiva.Value
                );

                // Actualizar el contrato con los nuevos meses adeudados
                rc.ActualizarTerminacionContrato(contrato.IdContrato, contrato.FechaTerminacionEfectiva.Value, mesesAdeudados);
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el pago.");
            TempData["ErrorMessage"] = "Ocurrió un error al procesar el pago.";
            return RedirectToAction(nameof(Index));
        }
    }


    public IActionResult Detalle(int idPago)
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
    [Authorize(Policy = "Administrador")]
    public IActionResult Eliminar(int id)
    {
        RepositorioPago rp = new RepositorioPago();
        RepositorioAuditoria ra = new RepositorioAuditoria();

        try
        {
            rp.EliminarPago(id, false);
            ra.RegistrarAuditoria("Anulacion de Pago", User.Identity.Name, $"Pago ID: {id}");
            TempData["SuccessMessage"] = "Estado del pago actualizado correctamente.";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            TempData["ErrorMessage"] = "No se pudo actualizar el estado del pago.";
        }
        return RedirectToAction(nameof(Index));
    }



    public IActionResult ListarPorContrato(int idContrato)
    {
        RepositorioPago rp = new RepositorioPago();
        RepositorioContrato rc = new RepositorioContrato();

        var pagos = rp.GetPagosPorContrato(idContrato);
        var contrato = rc.GetContrato(idContrato);
        if (contrato == null)
        {
            TempData["ErrorMessage"] = "El contrato no existe.";
            return RedirectToAction("Index");
        }

        ViewData["MontoAlquiler"] = contrato.MontoAlquiler > 0
            ? contrato.MontoAlquiler.ToString("0.##", CultureInfo.InvariantCulture)
            : "0";
        ViewData["IdContrato"] = idContrato;

        if (!pagos.Any())
        {
            ViewData["InfoMessage"] = "Este contrato no tiene pagos realizados.";
        }
        Console.WriteLine($"MontoAlquiler asignado: {ViewData["MontoAlquiler"]}");

        return View(pagos);
    }

    [HttpGet]
    public IActionResult GetPrecioPorContrato(int idContrato)
    {
        RepositorioContrato rc = new RepositorioContrato();
        var contrato = rc.GetContrato(idContrato);
        if (contrato != null && contrato.Inmueble != null)
        {
            return Json(new { precio = contrato.Inmueble.Valor });
        }
        return Json(new { precio = 0 });
    }


    [HttpPost]
    public IActionResult RegistrarTerminacion(int idContrato, DateTime fechaTerminacion)
    {
        RepositorioContrato rc = new RepositorioContrato();
        RepositorioPago rp = new RepositorioPago();

        rc.TerminarContrato(idContrato, fechaTerminacion);

        var contrato = rc.GetContrato(idContrato);
        if (contrato == null)
        {
            TempData["ErrorMessage"] = "El contrato no existe.";
            return RedirectToAction(nameof(Index));
        }

        var multa = contrato.CalcularMulta(fechaTerminacion);

        rp.RegistrarMulta(idContrato, multa);

        TempData["SuccessMessage"] = $"El contrato ha sido terminado anticipadamente. Multa registrada: ${multa}.";
        return RedirectToAction(nameof(Index));
    }

}