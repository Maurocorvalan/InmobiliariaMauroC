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

        // Verificar si hay un mensaje de éxito en TempData y pasarlo a la vista
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

        // Si idContrato está definido, cargar el contrato para edición
        if (idContrato.HasValue && idContrato > 0)
        {
            RepositorioContrato rc = new RepositorioContrato();
            var contrato = rc.GetContrato(idContrato.Value);
            return View(contrato);
        }
        else
        {
            // Crear un nuevo contrato con el inmueble preseleccionado si idInmueble está definido
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
            if (contrato.IdContrato > 0) // Contrato existente (Editar)
            {
                rc.ModificarContrato(contrato);
                TempData["SuccessMessage"] = "Contrato actualizado correctamente.";
            }
            else // Nuevo contrato (Crear)
            {
                rc.CrearContrato(contrato);
                TempData["SuccessMessage"] = "Contrato creado correctamente.";
            }

            // Redirigir al listado si todo salió bien
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex) // Superposición de fechas
        {
            TempData["ErrorMessage"] = "Fechas no disponibles para este Inmueble.";

            if (contrato.IdContrato > 0) // Contrato existente (Editar)
            {
                return RedirectToAction("Editar", new { idContrato = contrato.IdContrato });
            }
            else // Nuevo contrato (Crear)
            {
                return RedirectToAction(nameof(Crear));
            }
        }
        catch (Exception ex) // Otros errores
        {
            TempData["ErrorMessage"] = "Ocurrió un error inesperado: " + ex.Message;
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


}
