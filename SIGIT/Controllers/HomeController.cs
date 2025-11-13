using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SIGIT.Models;
using SIGIT.ViewModels; 
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace SIGIT.Controllers
{
    public class HomeController : Controller
    {
        private readonly SigitContext _context;

        public HomeController(SigitContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Si el usuario ya inició sesión, no le mostramos el login,
            // lo redirigimos a la página principal de la app (Listado de Empleados)
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Listado", "Empleados");
            }

            // Le pasamos un ViewModel vacío a la vista
            return View(new LoginViewModel());
        }

        // Procesa el formulario de login cuando el usuario hace clic en "Ingresar"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Buscar al usuario en la BD por su UsuarioLogin
                var user = await _context.UsuariosSistemas
                    .FirstOrDefaultAsync(u => u.UsuarioLogin == model.UsuarioLogin);

                // Verificar al usuario y la contraseña
                // (Validando la contraseña del formulario contra el campo PasswordHash)
                if (user != null && user.PasswordHash == model.Contrasena)
                {
                    // Crear la "identidad" del usuario (la cookie)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UsuarioLogin),
                        new Claim("FullName", $"{user.Nombre} {user.Apellido}"),
                        new Claim(ClaimTypes.Role, user.RolId.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    // Redirigir a la página principal de la app
                    return RedirectToAction("Listado", "Empleados");
                }
                else
                {
                    // Si el login falla, añade un error al modelo y muéstralo
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                }
            }

            // Si el modelo no es válido (ej. campos vacíos), vuelve a mostrar el formulario
            return View(model);
        }

        // Para cerrar sesión (se llama desde el botón "Salir" del Layout)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}