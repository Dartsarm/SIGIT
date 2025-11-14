using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGIT.Models;
using Microsoft.AspNetCore.Authorization; // <-- ¡Asegúrate de que este using esté!

namespace SIGIT.Controllers
{
    // AÑADIDO: Requiere que CUALQUIER usuario logueado acceda a este controlador
    [Authorize]
    public class CuentasEmpleadoesController : Controller
    {
        private readonly SigitContext _context;

        public CuentasEmpleadoesController(SigitContext context)
        {
            _context = context;
        }

        // GET: CuentasEmpleadoes (Todos los logueados pueden ver)
        public async Task<IActionResult> Index()
        {
            var sigitContext = _context.CuentasEmpleados.Include(c => c.Aplicacion).Include(c => c.Empleado);
            return View(await sigitContext.ToListAsync());
        }

        // GET: CuentasEmpleadoes/Details/5 (Todos los logueados pueden ver)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cuentasEmpleado = await _context.CuentasEmpleados
                .Include(c => c.Aplicacion)
                .Include(c => c.Empleado)
                .FirstOrDefaultAsync(m => m.CuentaId == id);
            if (cuentasEmpleado == null) return NotFound();

            return PartialView(cuentasEmpleado); // Devuelve vista parcial para el modal
        }

        // GET: CuentasEmpleadoes/Create
        // AÑADIDO: Solo Admin y Técnico
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return PartialView();
        }

        // POST: CuentasEmpleadoes/Create
        // AÑADIDO: Solo Admin y Técnico
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Create([Bind("EmpleadoId,AplicacionId,Usuario,Contrasena")] CuentasEmpleado cuentasEmpleado)
        {
            cuentasEmpleado.FechaCreacion = DateTime.Now;
            cuentasEmpleado.UltimaModificacion = DateTime.Now;
            ModelState.Remove("Empleado");
            ModelState.Remove("Aplicacion");
            ModelState.Remove("FechaCreacion");
            ModelState.Remove("UltimaModificacion");

            if (ModelState.IsValid)
            {
                _context.Add(cuentasEmpleado);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            await PopulateDropdowns(cuentasEmpleado);
            return PartialView(cuentasEmpleado);
        }

        // GET: CuentasEmpleadoes/Edit/5
        // AÑADIDO: Solo Admin y Técnico
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cuentasEmpleado = await _context.CuentasEmpleados.FindAsync(id);
            if (cuentasEmpleado == null) return NotFound();

            await PopulateDropdowns(cuentasEmpleado);
            return PartialView(cuentasEmpleado);
        }

        // POST: CuentasEmpleadoes/Edit/5
        // AÑADIDO: Solo Admin y Técnico
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Edit(int id, [Bind("CuentaId,EmpleadoId,AplicacionId,Usuario,Contrasena")] CuentasEmpleado cuentasEmpleado)
        {
            if (id != cuentasEmpleado.CuentaId) return NotFound();

            cuentasEmpleado.UltimaModificacion = DateTime.Now;
            ModelState.Remove("Empleado");
            ModelState.Remove("Aplicacion");
            ModelState.Remove("FechaCreacion");
            ModelState.Remove("UltimaModificacion");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuentasEmpleado);
                    _context.Entry(cuentasEmpleado).Property(x => x.FechaCreacion).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuentasEmpleadoExists(cuentasEmpleado.CuentaId)) return NotFound();
                    else throw;
                }
                return Json(new { success = true });
            }

            await PopulateDropdowns(cuentasEmpleado);
            return PartialView(cuentasEmpleado);
        }

        // GET: CuentasEmpleadoes/Delete/5
        // AÑADIDO: SOLO Admin
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cuentasEmpleado = await _context.CuentasEmpleados
                .Include(c => c.Aplicacion)
                .Include(c => c.Empleado)
                .FirstOrDefaultAsync(m => m.CuentaId == id);

            if (cuentasEmpleado == null) return NotFound();

            // CORREGIDO: Devuelve PartialView para el modal
            return PartialView(cuentasEmpleado);
        }

        // POST: CuentasEmpleadoes/Delete/5
        // AÑADIDO: SOLO Admin
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cuentasEmpleado = await _context.CuentasEmpleados.FindAsync(id);
            if (cuentasEmpleado != null)
            {
                _context.CuentasEmpleados.Remove(cuentasEmpleado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CuentasEmpleadoExists(int id)
        {
            return _context.CuentasEmpleados.Any(e => e.CuentaId == id);
        }

        // --- MÉTODO AYUDANTE PARA POBLAR DROPDOWNS ---
        private async Task PopulateDropdowns(CuentasEmpleado? cuentasEmpleado = null)
        {
            var aplicaciones = await _context.Aplicaciones.ToListAsync();

            var empleados = await _context.Empleados.ToListAsync();
            var empleadosList = empleados.Select(e => new {
                EmpleadoId = e.EmpleadoId,
                NombreCompleto = $"{e.Nombre} {e.SegundoNombre} {e.Apellido} {e.SegundoApellido}".Trim().Replace("  ", " ")
            }).ToList();

            if (cuentasEmpleado == null)
            {
                ViewBag.AplicacionId = new SelectList(aplicaciones, "AplicacionId", "NombreAplicacion");
                ViewBag.EmpleadoId = new SelectList(empleadosList, "EmpleadoId", "NombreCompleto");
            }
            else
            {
                ViewBag.AplicacionId = new SelectList(aplicaciones, "AplicacionId", "NombreAplicacion", cuentasEmpleado.AplicacionId);
                ViewBag.EmpleadoId = new SelectList(empleadosList, "EmpleadoId", "NombreCompleto", cuentasEmpleado.EmpleadoId);
            }
        }
    }
}