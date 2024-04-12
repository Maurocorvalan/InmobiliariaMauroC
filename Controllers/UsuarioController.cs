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
    [Authorize(Policy = "Administrador")]
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
    public IActionResult Editar(int idUsuario)
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
    public IActionResult EditarAvatar(Usuario usuario)
    {
        RepositorioUsuario ru = new RepositorioUsuario();
        if (usuario.IdUsuario > 0)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, regresar a la vista de edición con los errores
                return View(usuario);
            }

            try
            {
                // Guardar el nuevo avatar si se proporciona
                if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");

                    // Verificar si la carpeta "Uploads" existe, de lo contrario, crearla
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // Generar un nombre de archivo único para el avatar
                    string fileName = "avatar_" + usuario.IdUsuario + Path.GetExtension(usuario.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);

                    // Guardar el nuevo avatar
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        usuario.AvatarFile.CopyTo(stream);
                    }

                    // Actualizar la URL del avatar en el objeto usuario
                    usuario.AvatarUrl = Path.Combine("/Uploads", fileName);
                }

                // Actualizar la URL del avatar en la base de datos
                Console.WriteLine("id usuario  " + usuario.IdUsuario);
                ru.ModificarAvatar(usuario);
                TempData["SuccessMessage"] = "Avatar actualizado correctamente.";
                return RedirectToAction("Index");
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

                // Crear la lista de claims, incluyendo el IdUsuario
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
                new Claim(ClaimTypes.Role, usuario.RolNombre),
                new Claim("IdUsuario", usuario.IdUsuario.ToString()) // Agregar el IdUsuario como un claim
            };

                // Crear la identidad de claims
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Firmar al usuario
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

}




