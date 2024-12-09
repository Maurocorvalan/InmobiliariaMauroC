using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using System.Collections.Generic;

namespace Inmobiliaria.Controllers
{
    [Authorize(Policy = "Administrador")]
    public class AuditoriaController : Controller
    {
        private readonly RepositorioAuditoria repositorioAuditoria;

        public AuditoriaController()
        {
            repositorioAuditoria = new RepositorioAuditoria();
        }

        public IActionResult Index()
        {
            IList<Auditoria> auditorias = repositorioAuditoria.ObtenerAuditorias();
            return View(auditorias);
        }
    }
}
