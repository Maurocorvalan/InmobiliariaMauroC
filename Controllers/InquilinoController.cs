using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures; // Agregado para TempData
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly ILogger<InquilinoController> _logger;

        public InquilinoController(ILogger<InquilinoController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            RepositorioInquilino ri = new RepositorioInquilino();
            var lista = ri.GetInquilinos();

            if (TempData["SuccessMessage"] != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            }
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
                TempData["SuccessMessage"] = "Inquilino actualizado correctamente.";
            }
            else
            {
                ri.CrearInquilino(inquilino);
                TempData["SuccessMessage"] = "Inquilino creado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Eliminar(int id)
        {
            try{
            RepositorioInquilino ri = new RepositorioInquilino();
            ri.EliminarInquilino(id);
            TempData["SuccessMessage"] = "Inquilino eliminado correctamente.";
            }catch(Exception ex){
                TempData["SuccessMessage"] = "No se pudo eliminar el inquilino debido a que posee un contrato vigente";
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
