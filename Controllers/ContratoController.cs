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

        if (idContrato.HasValue && idContrato > 0)
        {
            RepositorioContrato rc = new RepositorioContrato();
            var contrato = rc.GetContrato(idContrato.Value);
            return View(contrato);
        }
        else
        {
            var nuevoContrato = new Contrato();
            if (idInmueble.HasValue)
            {
                nuevoContrato.IdInmueble = idInmueble.Value;
            }
            return View(nuevoContrato);
        }
    }

    [HttpPost]
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

        // Obtener los datos del contrato
        var contrato = rc.GetContrato(idContrato);
        if (contrato == null)
        {
            TempData["ErrorMessage"] = "El contrato no existe.";
            return RedirectToAction(nameof(Index));
        }

        // Calcular los pagos adeudados (hasta la fecha actual)
        int pagosAdeudados = rp.CalcularPagosAdeudados(idContrato, contrato.FechaInicio, contrato.FechaFinalizacion);

        // Pasar datos a la vista
        ViewBag.PagosAdeudados = pagosAdeudados;

        return View(contrato); // Pasa el contrato a la vista para mostrar detalles
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
