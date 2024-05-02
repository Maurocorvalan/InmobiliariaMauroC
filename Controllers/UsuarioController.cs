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

[Authorize]
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
    [Authorize(Policy = "Administrador")]
    public IActionResult Index()
    {
        if (!User.IsInRole("Administrador"))
        {
            return RedirectToAction("Index", "Home");
        }

        RepositorioUsuario ru = new RepositorioUsuario();
        var lista = ru.GetUsuarios();
        if (TempData["SuccessMessage"] != null)
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
        }
        return View(lista);
    }


    [HttpGet]
    [Authorize(Policy = "Administrador")]
    public IActionResult Crear()
    {
        if (User.IsInRole("empleado"))
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Roles = Usuario.ObtenerRoles();
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "Administrador")]

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
            TempData["SuccessMessage"] = "Usuario " + usuario.Nombre + " " + usuario.Apellido + " creado correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            Console.WriteLine(ex.Message);
            return View();
        }

    }
    [Authorize]


    public IActionResult Editar(int idUsuario)
    {
        if (User.IsInRole("empleado") && idUsuario.ToString() != User.FindFirst("IdUsuario")?.Value)
        {
            return Forbid();
        }
        RepositorioUsuario ru = new RepositorioUsuario();
        ViewBag.Usuarios = ru.GetUsuarios();
        if (idUsuario > 0)
        {
            var usuario = ru.GetUsuario(idUsuario);
            if (TempData["SuccessMessage"] != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            }
            else
            {
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }
            return View(usuario);
        }
        else
        {
            return View();
        }

    }
    [Authorize]


    public IActionResult EditarAvatar(Usuario usuario)
    {
        if (User.IsInRole("empleado") && usuario.IdUsuario.ToString() != User.FindFirst("IdUsuario")?.Value)
        {
            return Forbid();
        }
        RepositorioUsuario ru = new RepositorioUsuario();
        if (usuario.IdUsuario > 0)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            try
            {
                if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = "avatar_" + usuario.IdUsuario + Path.GetExtension(usuario.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);

                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        usuario.AvatarFile.CopyTo(stream);
                    }

                    usuario.AvatarUrl = Path.Combine("/Uploads", fileName);
                }

                ru.ModificarAvatar(usuario);
                TempData["SuccessMessage"] = "Avatar actualizado correctamente.";
                if (User.IsInRole("Empleado"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                Console.WriteLine(ex.Message);
                return View(usuario);
            }
        }
        else
        {
            return View();
        }
    }
    [Authorize]

    public IActionResult CambiarClave(Usuario usuario)
    {
        if (User.IsInRole("empleado") && usuario.IdUsuario.ToString() != User.FindFirst("IdUsuario")?.Value)
        {
            return Forbid();
        }
        string claveActual = Request.Form["claveActual"];
        string claveNueva = Request.Form["claveNueva"];
        string claveConfirmar = Request.Form["claveConfirmar"];
        RepositorioUsuario ru = new RepositorioUsuario();
        var user = ru.ObtenerPorId(usuario.IdUsuario);
        string hashed = HashPassword(claveActual);
        try
        {
            if (hashed != user.Clave)
            {
                Console.WriteLine("Contraseña actual incorrecta puesta");
                TempData["ErrorMessage"] = "Contraseña actual incorrecta.";
                return RedirectToAction("Editar", new { IdUsuario = usuario.IdUsuario });

            }
            else if (claveNueva != claveConfirmar)
            {
                Console.WriteLine("Las claves no coinciden");
                TempData["ErrorMessage"] = "Las contraseñas no coinciden.";

                return RedirectToAction("Editar", new { IdUsuario = usuario.IdUsuario });

            }
            else
            {
                string nuevaHasheada = HashPassword(claveNueva);
                user.Clave = nuevaHasheada;
                ru.ModificarClave(user);
                TempData["SuccessMessage"] = "Clave cambiada correctamente.";
                return RedirectToAction("Editar", new { IdUsuario = usuario.IdUsuario });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return RedirectToAction("Index");
    }
    [Authorize(Policy = "Administrador")]

    public IActionResult Eliminar(int id)
    {
        try
        {
            RepositorioUsuario ru = new RepositorioUsuario();
            ru.EliminarUsuario(id);
            TempData["SuccessMessage"] = "Usuario eliminado correctamente.";

        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "No se pudo eliminar el usuario";
            Console.WriteLine(ex.Message);
        }
        return RedirectToAction(nameof(Index));
    }
    [Authorize]
    public IActionResult EditarDatos(Usuario usuario)
    {
          if (User.IsInRole("empleado") && usuario.IdUsuario.ToString() != User.FindFirst("IdUsuario")?.Value)
        {
            return Forbid();
        }
        string claveActual = Request.Form["claveActual"];
        RepositorioUsuario ru = new RepositorioUsuario();
        var user = ru.ObtenerPorId(usuario.IdUsuario);
        string hashed = HashPassword(claveActual);
        try
        {

            if (hashed != user.Clave)
            {
                TempData["ErrorMessage"] = "Error al editar datos";
                return RedirectToAction("Editar", new { IdUsuario = usuario.IdUsuario });
            }
            else
            {
                user.Nombre = Request.Form["Nombre"];
                user.Email = Request.Form["Email"];
                user.Apellido = Request.Form["Apellido"];
                ru.ModificarDatos(user);
                TempData["SuccessMessage"] = "Datos cambiados correctamente.";
                return RedirectToAction("Editar", new { IdUsuario = usuario.IdUsuario });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

        }
        return RedirectToAction("Index");
    }
    public IActionResult Detalle(int idUsuario)
    {
        RepositorioUsuario ru = new RepositorioUsuario();
        ViewBag.Usuarios = ru.GetUsuarios();
        if (idUsuario > 0)
        {
            var usuario = ru.GetUsuario(idUsuario);
            return View(usuario);
        }
        else
        {

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
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
                new Claim(ClaimTypes.Role, usuario.RolNombre),
                new Claim("IdUsuario", usuario.IdUsuario.ToString())
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

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
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
    private string UnhashPassword(string hashedPassword)
    {
        byte[] hashedBytes = Convert.FromBase64String(hashedPassword);
        string password = System.Text.Encoding.UTF8.GetString(hashedBytes);

        return password;
    }



}




