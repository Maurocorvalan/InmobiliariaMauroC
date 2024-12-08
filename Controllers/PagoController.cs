using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;

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
            // Obtener los contratos para mostrarlos en la vista
            RepositorioContrato rc = new RepositorioContrato();
            ViewBag.Contratos = rc.GetContratos();

            if (idPago > 0)
            {
                // Obtener el pago específico
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
            // Manejo de errores
            _logger.LogError(ex, "Error al cargar la vista de edición.");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar cargar la edición.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult Guardar(Pago pago)
    {
        RepositorioPago rp = new RepositorioPago();
        try
        {
            if (pago.IdPago > 0)
            {
                rp.ModificarPago(pago);
                TempData["SuccessMessage"] = "Pago actualizado correctamente.";
            }
            else
            {
                rp.CrearPago(pago);
                TempData["SuccessMessage"] = "Pago creado correctamente.";
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
        try
        {
            rp.EliminarPago(id, false);
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
        var pagos = rp.GetPagosPorContrato(idContrato); // Método a implementar en el repositorio

        ViewData["IdContrato"] = idContrato;

        if (!pagos.Any())
        {
            ViewData["InfoMessage"] = "Este contrato no tiene pagos realizados.";
        }

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

        // Registrar la fecha de terminación anticipada
        rc.TerminarContrato(idContrato, fechaTerminacion);

        // Recuperar el contrato actualizado (con la fecha de terminación efectiva registrada)
        var contrato = rc.GetContrato(idContrato);
        if (contrato == null)
        {
            TempData["ErrorMessage"] = "El contrato no existe.";
            return RedirectToAction(nameof(Index));
        }

        // Calcular la multa basada en la fecha de terminación ingresada
        var multa = contrato.CalcularMulta(fechaTerminacion);

        // Registrar la multa como un pago pendiente con el detalle adecuado
        rp.RegistrarMulta(idContrato, multa);

        TempData["SuccessMessage"] = $"El contrato ha sido terminado anticipadamente. Multa registrada: ${multa}.";
        return RedirectToAction(nameof(Index));
    }

}