using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Org.BouncyCastle.Asn1.Iana;
using Microsoft.AspNetCore.Authorization;
namespace Inmobiliaria.Controllers;
[Authorize]
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

        if (TempData["SuccessMessage"] != null)
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
        }

        return View(lista);
    }

    [HttpGet]
    public IActionResult Crear(int? idContrato, int? idInmueble)
    {
        RepositorioInquilino ri = new RepositorioInquilino();
        RepositorioInmueble rim = new RepositorioInmueble();

        ViewBag.Inquilinos = ri.GetInquilinos();
        ViewBag.Inmuebles = rim.GetInmuebles();
        RepositorioContrato rc = new RepositorioContrato();

        if (idContrato.HasValue && idContrato > 0)
        {
            var contrato = rc.GetContrato(idContrato.Value);
            return View(contrato);
        }
        else
        {
            var nuevoContrato = new Contrato();
            if (idInmueble.HasValue)
            {
                nuevoContrato.IdInmueble = idInmueble.Value;
                var inmuebleSeleccionado = rim.GetInmueble(idInmueble.Value);
                if (inmuebleSeleccionado != null)
                {
                    nuevoContrato.MontoAlquiler = inmuebleSeleccionado.Valor;
                }
            }
            return View(nuevoContrato);
        }
    }

    [HttpPost]
    public IActionResult Guardar(Contrato contrato)
    {
        RepositorioContrato rc = new RepositorioContrato();
        RepositorioAuditoria ra = new RepositorioAuditoria();

        try
        {
            if (contrato.IdContrato > 0)
            {
                rc.ModificarContrato(contrato);
                ra.RegistrarAuditoria("Modificación Contrato", User.Identity.Name, $"Contrato ID: {contrato.IdContrato}");
                TempData["SuccessMessage"] = "Contrato actualizado correctamente.";
            }
            else
            {
                rc.CrearContrato(contrato);
                ra.RegistrarAuditoria("Creación Contrato", User.Identity.Name, $"Inmueble ID: {contrato.IdInmueble}");
                TempData["SuccessMessage"] = "Contrato creado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = "Fechas no disponibles para este Inmueble.";

            if (contrato.IdContrato > 0)
            {
                return RedirectToAction("Editar", new { idContrato = contrato.IdContrato });
            }
            else
            {
                return RedirectToAction(nameof(Crear));
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Fechas no disponibles para este inmueble: Fecha inicio:{contrato.FechaInicio} - Fecha Finalizacion:{contrato.FechaFinalizacion}";

            if (Request.Form["EsRenovar"] == "true")
            {
                var idContratoOriginal = Request.Form["idContratoOriginal"];

                return RedirectToAction("Renovar", new { idContrato = idContratoOriginal });
            }
            return RedirectToAction(nameof(Index));
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

    public IActionResult Detalle(int idContrato)
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
    [Authorize(Policy = "Administrador")]


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

    public IActionResult ContratosVigentes()
    {
        RepositorioContrato rc = new RepositorioContrato();
        var contratosVigentes = rc.GetContratosVigentes();

        if (TempData["SuccessMessage"] != null)
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
        }

        return View(contratosVigentes);
    }





    public IActionResult ListarPorInmueble(int idInmueble)
    {
        RepositorioContrato rc = new RepositorioContrato();
        var contratos = rc.GetContratosPorInmueble(idInmueble);

        if (contratos == null || !contratos.Any())
        {
            ViewData["NoContractsMessage"] = "Este inmueble no posee ningún contrato.";
            return View(new List<Contrato>());
        }

        ViewBag.IdInmueble = idInmueble;
        return View(contratos);
    }


    [HttpGet]
    public IActionResult Renovar(int idContrato)
    {
        RepositorioContrato rc = new RepositorioContrato();
        var contratoActual = rc.GetContrato(idContrato);

        if (contratoActual == null)
        {
            TempData["ErrorMessage"] = "El contrato no existe.";
            return RedirectToAction(nameof(Index));
        }

        var nuevoContrato = new Contrato
        {
            IdContrato = idContrato,
            IdInquilino = contratoActual.IdInquilino,
            IdInmueble = contratoActual.IdInmueble,
            Inquilino = contratoActual.Inquilino,
            Inmueble = contratoActual.Inmueble,
            FechaInicio = DateTime.Now,
            FechaFinalizacion = DateTime.Now.AddYears(1),
            MontoAlquiler = contratoActual.MontoAlquiler
        };

        return View("Renovar", nuevoContrato);
    }

    [HttpGet]
    public IActionResult TerminarContrato(int idContrato)
    {
        RepositorioContrato rc = new RepositorioContrato();
        RepositorioPago rp = new RepositorioPago();

        var contrato = rc.GetContrato(idContrato);
        if (contrato == null)
        {
            TempData["ErrorMessage"] = "El contrato no existe.";
            return RedirectToAction(nameof(Index));
        }

        int pagosAdeudados = rp.CalcularPagosAdeudados(idContrato, contrato.FechaInicio, DateTime.Now);

        ViewBag.PagosAdeudados = pagosAdeudados;
        ViewBag.FechaInicio = contrato.FechaInicio;

        return View(contrato);
    }
    [HttpPost]
    public IActionResult RegistrarTerminacion(int idContrato, DateTime fechaTerminacion)
    {
        RepositorioContrato rc = new RepositorioContrato();
        RepositorioPago rp = new RepositorioPago();
        RepositorioAuditoria ra = new RepositorioAuditoria();

        try
        {
            var contrato = rc.GetContrato(idContrato);
            if (contrato == null)
            {
                TempData["ErrorMessage"] = "El contrato no existe.";
                return RedirectToAction(nameof(Index));
            }

            int mesesAdeudados = rp.CalcularPagosAdeudados(idContrato, contrato.FechaInicio, fechaTerminacion);

            rc.ActualizarTerminacionContrato(idContrato, fechaTerminacion, mesesAdeudados);

            var multa = contrato.CalcularMulta(fechaTerminacion);

            rp.RegistrarMulta(idContrato, multa);



            ra.RegistrarAuditoria("Terminación Contrato", User.Identity.Name, $"Contrato ID: {idContrato}, Fecha Terminación: {fechaTerminacion}, Meses Adeudados: {mesesAdeudados}, Multa: {multa}");

            TempData["SuccessMessage"] = $"El contrato ha sido terminado anticipadamente. Multa registrada: ${multa}. Meses adeudados: {mesesAdeudados}.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al terminar el contrato: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpGet]
    public IActionResult BuscarPorRango(DateTime fechaLimite)
    {
        var fechaInicio = DateTime.Now.Date;
        RepositorioContrato rc = new RepositorioContrato();

        var contratos = rc.GetContratosPorRango(fechaInicio, fechaLimite);

        if (!contratos.Any())
        {
        }

        return View("BuscarPorRango", contratos);
    }
    [HttpPost]
    public IActionResult CalcularMesesAdeudados(int idContrato, DateTime fechaTerminacion)
    {
        RepositorioContrato rc = new RepositorioContrato();
        RepositorioPago rp = new RepositorioPago();

        var contrato = rc.GetContrato(idContrato);
        if (contrato == null)
        {
            return Json(new { error = "El contrato no existe" });
        }

        int mesesAdeudados = rp.CalcularPagosAdeudados(idContrato, contrato.FechaInicio, fechaTerminacion);
        return Json(new { mesesAdeudados });
    }
    [HttpGet]
    public IActionResult ContratosFinalizados()
    {
        RepositorioContrato rc = new RepositorioContrato();
        var contratosFinalizados = rc.GetContratosFinalizados();

        if (!contratosFinalizados.Any())
        {
            ViewData["InfoMessage"] = "No se encontraron contratos finalizados.";
        }

        return View(contratosFinalizados);
    }


}
