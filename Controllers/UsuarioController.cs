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

    public UsuarioController(ILogger<UsuarioController> logger, IConfiguration configuration, IWebHostEnvironment environment, RepositorioUsuario repositorioUsuario)
    {
        _logger = logger;
        this.configuration = configuration;
        this.environment = environment;
        this.repositorioUsuario = repositorioUsuario;
    }

    public IActionResult Index()
    {
        RepositorioUsuario ru = new RepositorioUsuario();
        var lista = ru.GetUsuarios();
        if (TempData["SuccessMessage"] != null)
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];

        }
        return View(lista);
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
            usuario.Clave = HashPassword(usuario.Clave);
            var nombreRandom = Guid.NewGuid();
            int res = repositorioUsuario.CrearUsuario(usuario);
            if (usuario.AvatarFile != null && usuario.IdUsuario > 0)
            {
                string wwwPath = environment.WebRootPath;
                string path = Path.Combine(wwwPath, "Avatars");
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
            TempData["SuccessMessage"] = "Usuario creado correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            Console.WriteLine(ex.Message);
            return View();
        }

    }
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
        TempData["returnUrl"] = returnUrl;
        return View();
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginView login)
    {
        try
        {
            var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
            if (ModelState.IsValid)
            {
                string hashed = HashPassword(login.Clave);
                var usuario = repositorioUsuario.ObtenerPorEmail(login.Email);
                if (usuario == null || usuario.Clave != hashed)
                {
                    ModelState.AddModelError("", "El email o la clave no son correctos");
                    TempData["returnUrl"] = returnUrl;


                    return View();
                }else{
                    Console.WriteLine("logueado");
                }

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Email),
                        new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
                        new Claim(ClaimTypes.Role, usuario.RolNombre),
                    };

                var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                        Console.WriteLine("User signed in successfully.");

                TempData.Remove("returnUrl");
                Console.WriteLine($"Redirecting to: {returnUrl}");

                return Redirect(returnUrl);
            }
            TempData["returnUrl"] = returnUrl;
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }

    //funcion para hashear la contraseña
    private string HashPassword(string password)
    {
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return hashed;
    }

}




