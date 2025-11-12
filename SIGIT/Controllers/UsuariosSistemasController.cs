using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGIT.Models;

namespace SIGIT.Controllers
{
    public class UsuariosSistemasController : Controller
    {
        private readonly SigitContext _context;

        public UsuariosSistemasController(SigitContext context)
        {
            _context = context;
        }

        // GET: UsuariosSistemas
        public async Task<IActionResult> Index()
        {
            var sigitContext = _context.UsuariosSistemas.Include(u => u.Rol);
            return View(await sigitContext.ToListAsync());
        }

        // GET: UsuariosSistemas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuariosSistema = await _context.UsuariosSistemas
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioSistemaId == id);

            if (usuariosSistema == null)
            {
                return NotFound();
            }

            return PartialView(usuariosSistema);
        }

        // GET: UsuariosSistemas/Create
        public async Task<IActionResult> Create()
        {
            // CORREGIDO: Llama al método para poblar los dropdowns
            await PopulateDropdowns();
            return PartialView();
        }

        // POST: UsuariosSistemas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioSistemaId,Cedula,Nombre,SegundoNombre,Apellido,SegundoApellido,UsuarioLogin,Email,Celular,PasswordHash,RolId,Activo")] UsuariosSistema usuariosSistema)
        {
            // CORREGIDO: Asignar fechas y remover validaciones de servidor
            usuariosSistema.FechaRegistro = DateTime.Now;
            ModelState.Remove("FechaRegistro");
            ModelState.Remove("Rol"); // Ignorar el objeto de navegación nulo

            if (ModelState.IsValid)
            {
                // Aquí deberías HASH la contraseña antes de guardar
                // Ejemplo: usuariosSistema.PasswordHash = HashPassword(usuariosSistema.PasswordHash);

                _context.Add(usuariosSistema);
                await _context.SaveChangesAsync();
                return Json(new { success = true }); // Devuelve Éxito
            }

            // Si hay error, repoblar dropdowns y devolver vista con errores
            await PopulateDropdowns(usuariosSistema);
            return PartialView(usuariosSistema);
        }

        // GET: UsuariosSistemas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuariosSistema = await _context.UsuariosSistemas.FindAsync(id);
            if (usuariosSistema == null)
            {
                return NotFound();
            }

            // CORREGIDO: Llama al método para poblar los dropdowns
            await PopulateDropdowns(usuariosSistema);
            return PartialView(usuariosSistema);
        }

        // POST: UsuariosSistemas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioSistemaId,Cedula,Nombre,SegundoNombre,Apellido,SegundoApellido,UsuarioLogin,Email,Celular,PasswordHash,RolId,Activo")] UsuariosSistema usuariosSistema)
        {
            if (id != usuariosSistema.UsuarioSistemaId)
            {
                return NotFound();
            }

            // CORREGIDO: Remover validaciones de servidor
            ModelState.Remove("FechaRegistro");
            ModelState.Remove("Rol");

            if (ModelState.IsValid)
            {
                try
                {
                    // Aquí deberías verificar si la contraseña cambió para volver a Hashearla
                    // if (PasswordHaCambiado) {
                    //    usuariosSistema.PasswordHash = HashPassword(usuariosSistema.PasswordHash);
                    // }

                    _context.Update(usuariosSistema);

                    // Proteger FechaRegistro para que no se modifique
                    _context.Entry(usuariosSistema).Property(x => x.FechaRegistro).IsModified = false;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuariosSistemaExists(usuariosSistema.UsuarioSistemaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new { success = true });
            }

            // Si hay error, repoblar dropdowns y devolver vista con errores
            await PopulateDropdowns(usuariosSistema);
            return PartialView(usuariosSistema);
        }

        // GET: UsuariosSistemas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuariosSistema = await _context.UsuariosSistemas
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioSistemaId == id);

            if (usuariosSistema == null)
            {
                return NotFound();
            }

            return View(usuariosSistema);
        }

        // POST: UsuariosSistemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuariosSistema = await _context.UsuariosSistemas.FindAsync(id);
            if (usuariosSistema != null)
            {
                _context.UsuariosSistemas.Remove(usuariosSistema);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuariosSistemaExists(int id)
        {
            return _context.UsuariosSistemas.Any(e => e.UsuarioSistemaId == id);
        }

        // --- MÉTODO AYUDANTE PARA POBLAR DROPDOWNS ---
        private async Task PopulateDropdowns(UsuariosSistema? usuariosSistema = null)
        {
            var roles = await _context.Roles.ToListAsync();

            if (usuariosSistema == null)
            {
                // CORREGIDO: Cargar "NombreRol"
                ViewBag.RolId = new SelectList(roles, "RolId", "NombreRol");
            }
            else
            {
                // CORREGIDO: Cargar "NombreRol" y seleccionar el actual
                ViewBag.RolId = new SelectList(roles, "RolId", "NombreRol", usuariosSistema.RolId);
            }
        }
    }
}