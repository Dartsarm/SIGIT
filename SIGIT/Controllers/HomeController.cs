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
            // ... (código del Index GET) ...
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Listado", "Empleados");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            // ... (código del Index POST) ...
            if (ModelState.IsValid)
            {
                var user = await _context.UsuariosSistemas
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.UsuarioLogin == model.UsuarioLogin);

                if (user != null && user.PasswordHash == model.Contrasena)
                {
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, user.UsuarioLogin),
                        new Claim("FullName", $"{user.Nombre} {user.Apellido}"),
                        new Claim(ClaimTypes.Role, user.Rol.NombreRol)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Listado", "Empleados");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrecta");
                }
            }
            return View(model);
        }

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
             
        public IActionResult AccessDenied()
        {
            // Devuelve una vista parcial para que se cargue limpiamente en el modal
            return PartialView();
        }
    } 
} 