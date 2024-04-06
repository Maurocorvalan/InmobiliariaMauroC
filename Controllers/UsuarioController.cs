using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Security;


public class UsuarioController : Controller
{
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;
    private readonly RepositorioUsuario repositorioUsuario;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _logger = logger;
        this.configuration = configuration;
        this.environment = environment;
    }

    public IActionResult Index()
    {
        RepositorioUsuario ru = new RepositorioUsuario();
        if (TempData["SuccessMessage"] != null)
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];

        }
        return View();
    }

    [HttpGet]
    public IActionResult Crear()
    {
        ViewBag.Roles = Usuario.ObtenerRoles();
        return View();
    }
    [HttpPost]
    public IActionResult Crear(Usuario usuario)
    {
        if (!ModelState.IsValid)

            return View();
        try
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                 password: usuario.Clave,
                 salt: System.Text.Encoding.ASCII.GetBytes(configuration["SALADA"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            usuario.Clave = hashed;
            var nombreRandom = Guid.NewGuid();
            int ru = repositorioUsuario.CrearUsuario(usuario);
            if (usuario.AvatarFile != null && usuario.IdUsuario > 0)
            {
                string wwwPath = environment.WebRootPath;
                string path = Path.Combine(wwwPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = "avatar_" + usuario.IdUsuario + Path.GetExtension(usuario.AvatarFile.FileName);
                string pathCompleto = Path.Combine(path, fileName);
                usuario.AvatarUrl = Path.Combine("/Uploads", fileName);
                using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                {
                    usuario.AvatarFile.CopyTo(stream);
                }
                repositorioUsuario.ModificarUsuario(usuario);
            }
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

    }
    public IActionResult Guardar(Usuario usuario)
    {
        RepositorioUsuario ru = new RepositorioUsuario();
        try
        {
            if (usuario.IdUsuario > 0)
            {
                ru.ModificarUsuario(usuario);
                TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
            }
            else
            {
                ru.CrearUsuario(usuario);
                TempData["SuccessMessage"] = "Usuario creado correctamente.";
            }
            return RedirectToAction("Index");

        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al crear usuario";
            return RedirectToAction(nameof(Crear));
            Console.WriteLine(ex.Message);

        }
    }
}



