using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGIT.Models;
using System.Data.Common;

namespace SIGIT.Controllers
{
    
    [Authorize(Roles = "Administrador, Técnico, Usuario")]
    public class AreasController : Controller
    {
        private readonly SigitContext _context;

        public AreasController(SigitContext context)
        {
            _context = context;
        }

        // GET: Areas (Todos pueden ver)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Areas.ToListAsync());
        }

        // GET: Areas/Details/5 (Todos pueden ver)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var area = await _context.Areas.FirstOrDefaultAsync(m => m.AreaId == id);
            if (area == null) return NotFound();
            return PartialView(area);
        }

        // GET: Areas/Create (Solo Admin y Técnico)
        [Authorize(Roles = "Administrador, Técnico")]
        public IActionResult Create()
        {
            return PartialView();
        }

        // POST: Areas/Create (Solo Admin y Técnico)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Create([Bind("AreaId,NombreArea")] Area area)
        {
            if (ModelState.IsValid)
            {
                _context.Add(area);
                await _context.SaveChangesAsync();
                return Json(new { success = true }); // Devuelve JSON de éxito
            }
            return PartialView(area);
        }

        // GET: Areas/Edit/5 (Admin y Técnico)
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return NotFound();
            return PartialView(area);
        }

        // POST: Areas/Edit/5 (Admin y Técnico)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Edit(int id, [Bind("AreaId,NombreArea")] Area area)
        {
            if (id != area.AreaId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(area);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(area.AreaId)) return NotFound();
                    else throw;
                }
                return Json(new { success = true }); // Devuelve JSON de éxito
            }
            return PartialView(area);
        }

        // GET: Areas/Delete/5 (Solo Admin)
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var area = await _context.Areas.FirstOrDefaultAsync(m => m.AreaId == id);
            if (area == null) return NotFound();
            return PartialView(area);
        }

        // POST: Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var area = await _context.Areas.FindAsync(id);

            try
            {
                if (area != null)
                {
                    _context.Areas.Remove(area);
                    await _context.SaveChangesAsync();
                }
                return Json(new { success = true });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is DbException dbEx && (dbEx.Message.Contains("REFERENCE constraint") || dbEx.Message.Contains("FK_")))
                {
                    return Json(new { success = false, message = "No se puede eliminar esta Área porque está siendo utilizada por uno o más empleados. Elimine o reasigne los empleados primero." });
                }

                return Json(new { success = false, message = $"Error de servidor inesperado: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error desconocido: {ex.Message}" });
            }
        }

        private bool AreaExists(int id)
        {
            return _context.Areas.Any(e => e.AreaId == id);
        }
    }
}