using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures; // Agregado para TempData
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly ILogger<PropietarioController> _logger;

        public PropietarioController(ILogger<PropietarioController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            RepositorioPropietarios rp = new RepositorioPropietarios();
            var lista = rp.GetPropietarios();

            if (TempData["SuccessMessage"] != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            }

            return View(lista);
        }

        [HttpGet]
        public IActionResult Editar(int idPropietario)
        {
            if (idPropietario > 0)
            {
                RepositorioPropietarios rp = new RepositorioPropietarios();
                var propietario = rp.GetPropietario(idPropietario);
                return View(propietario);
            }
            else
            {
                return View();
            }
        }

      public IActionResult Detalle(int idPropietario)
        {
            if(idPropietario > 0){
                RepositorioPropietarios rp = new RepositorioPropietarios();
                var propietario = rp.GetPropietario(idPropietario);
                return View(propietario);
            }else{
                return View();
            }
        }




        [HttpPost]
        public IActionResult Guardar(Propietario propietario)
        {
            RepositorioPropietarios rp = new RepositorioPropietarios();
            if (propietario.IdPropietario > 0)
            {
                rp.ModificarPropietario(propietario);
                TempData["SuccessMessage"] = "Propietario actualizado correctamente.";
            }
            else
            {
                rp.CrearPropietario(propietario);
                TempData["SuccessMessage"] = "Propietario creado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

  
        public IActionResult Eliminar(int id)
        {
            try{
            RepositorioPropietarios rp = new RepositorioPropietarios();
            rp.EliminarPropietario(id);
            TempData["SuccessMessage"] = "Propietario eliminado correctamente.";
            }catch(Exception ex){
                TempData["SuccessMessage"] = "No se pudo eliminar el propietario debido a que posee un inmueble o contrato vigente";
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
