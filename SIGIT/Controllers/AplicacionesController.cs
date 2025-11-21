using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGIT.Models;
using Microsoft.AspNetCore.Authorization; 

namespace SIGIT.Controllers
{
    [Authorize]
    public class AplicacionesController : Controller
    {
        private readonly SigitContext _context;

        public AplicacionesController(SigitContext context)
        {
            _context = context;
        }

        // GET: Aplicaciones
        public async Task<IActionResult> Index()
        {
            return View(await _context.Aplicaciones.ToListAsync());
        }

        // GET: Aplicaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var aplicacione = await _context.Aplicaciones.FirstOrDefaultAsync(m => m.AplicacionId == id);
            if (aplicacione == null) return NotFound();

            return PartialView(aplicacione);
        }

        // GET: Aplicaciones/Create
        [Authorize(Roles = "Administrador")] // Solo el administrador puede crear
        public IActionResult Create()
        {
            
            return PartialView();
        }

        // POST: Aplicaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("AplicacionId,NombreAplicacion")] Aplicacione aplicacione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aplicacione);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView(aplicacione);
        }

        // GET: Aplicaciones/Edit/5
        [Authorize(Roles = "Administrador, Técnico")] // Admin y Técnico pueden editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var aplicacione = await _context.Aplicaciones.FindAsync(id);
            if (aplicacione == null) return NotFound();

            
            return PartialView(aplicacione);
        }

        // POST: Aplicaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Edit(int id, [Bind("AplicacionId,NombreAplicacion")] Aplicacione aplicacione)
        {
            if (id != aplicacione.AplicacionId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aplicacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AplicacioneExists(aplicacione.AplicacionId)) return NotFound();
                    else throw;
                }
                return Json(new { success = true });
            }
            return PartialView(aplicacione);
        }

        // GET: Aplicaciones/Delete/5
        [Authorize(Roles = "Administrador")] // Solo el administrador puede eliminar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var aplicacione = await _context.Aplicaciones.FirstOrDefaultAsync(m => m.AplicacionId == id);
            if (aplicacione == null) return NotFound();

            return PartialView(aplicacione); // Devuelve PartialView
        }

        // POST: Aplicaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aplicacione = await _context.Aplicaciones.FindAsync(id);
            if (aplicacione != null)
            {
                _context.Aplicaciones.Remove(aplicacione);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true }); // Devuelve JSON para el script del modal
        }

        private bool AplicacioneExists(int id)
        {
            return _context.Aplicaciones.Any(e => e.AplicacionId == id);
        }
    }
}