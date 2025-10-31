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
    public class CuentasEmpleadoesController : Controller
    {
        private readonly SigitContext _context;

        public CuentasEmpleadoesController(SigitContext context)
        {
            _context = context;
        }

        // GET: CuentasEmpleadoes
        public async Task<IActionResult> Index()
        {
            var sigitContext = _context.CuentasEmpleados.Include(c => c.Aplicacion).Include(c => c.Empleado);
            return View(await sigitContext.ToListAsync());
        }

        // GET: CuentasEmpleadoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentasEmpleado = await _context.CuentasEmpleados
                .Include(c => c.Aplicacion)
                .Include(c => c.Empleado)
                .FirstOrDefaultAsync(m => m.CuentaId == id);

            if (cuentasEmpleado == null)
            {
                return NotFound();
            }

            // Devuelve vista parcial para el modal
            return PartialView(cuentasEmpleado);
        }

        // GET: CuentasEmpleadoes/Create
        public async Task<IActionResult> Create()
        {
            // Llama al método para poblar los dropdowns vacíos
            await PopulateDropdowns();
            // Devuelve Vista Parcial para el modal
            return PartialView();
        }

        // POST: CuentasEmpleadoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpleadoId,AplicacionId,Usuario,Contrasena")] CuentasEmpleado cuentasEmpleado)
        {
            // 1. Asigna las fechas del servidor
            cuentasEmpleado.FechaCreacion = DateTime.Now;
            cuentasEmpleado.UltimaModificacion = DateTime.Now;

            // 2. Quita los errores de ModelState para campos que no vienen del formulario
            ModelState.Remove("Empleado");
            ModelState.Remove("Aplicacion");
            ModelState.Remove("FechaCreacion");
            ModelState.Remove("UltimaModificacion");

            if (ModelState.IsValid)
            {
                _context.Add(cuentasEmpleado);
                await _context.SaveChangesAsync();
                return Json(new { success = true }); // Devuelve ÉXITO
            }

            // Si hay error, repoblar dropdowns y devolver vista con errores
            await PopulateDropdowns(cuentasEmpleado);
            return PartialView(cuentasEmpleado);
        }

        // GET: CuentasEmpleadoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentasEmpleado = await _context.CuentasEmpleados.FindAsync(id);
            if (cuentasEmpleado == null)
            {
                return NotFound();
            }

            // Llama al método para poblar los dropdowns con valores seleccionados
            await PopulateDropdowns(cuentasEmpleado);
            // Devuelve Vista Parcial para el modal
            return PartialView(cuentasEmpleado);
        }

        // POST: CuentasEmpleadoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CuentaId,EmpleadoId,AplicacionId,Usuario,Contrasena")] CuentasEmpleado cuentasEmpleado)
        {
            if (id != cuentasEmpleado.CuentaId)
            {
                return NotFound();
            }

            // 1. Asigna la fecha de modificación del servidor
            cuentasEmpleado.UltimaModificacion = DateTime.Now;

            // 2. Quita los errores de ModelState
            ModelState.Remove("Empleado");
            ModelState.Remove("Aplicacion");
            ModelState.Remove("FechaCreacion"); // No se debe editar
            ModelState.Remove("UltimaModificacion");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuentasEmpleado);

                    // Le decimos a EF que NO MODIFIQUE el campo FechaCreacion
                    _context.Entry(cuentasEmpleado).Property(x => x.FechaCreacion).IsModified = false;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuentasEmpleadoExists(cuentasEmpleado.CuentaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new { success = true }); // Devuelve ÉXITO
            }

            // Si hay error, repoblar dropdowns y devolver vista con errores
            await PopulateDropdowns(cuentasEmpleado);
            return PartialView(cuentasEmpleado);
        }

        // GET: CuentasEmpleadoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentasEmpleado = await _context.CuentasEmpleados
                .Include(c => c.Aplicacion)
                .Include(c => c.Empleado)
                .FirstOrDefaultAsync(m => m.CuentaId == id);

            if (cuentasEmpleado == null)
            {
                return NotFound();
            }

            return View(cuentasEmpleado);
        }

        // POST: CuentasEmpleadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
            // 1. Cargar Aplicaciones (Sencillo)
            var aplicaciones = await _context.Aplicaciones.ToListAsync();

            // 2. Cargar Empleados y CONCATENAR NOMBRES
            var empleados = await _context.Empleados.ToListAsync();
            var empleadosList = empleados.Select(e => new {
                EmpleadoId = e.EmpleadoId,
                NombreCompleto = $"{e.Nombre} {e.SegundoNombre} {e.Apellido} {e.SegundoApellido}".Trim().Replace("  ", " ")
            }).ToList();

            if (cuentasEmpleado == null)
            {
                // Para Create (GET) - sin selección
                ViewBag.AplicacionId = new SelectList(aplicaciones, "AplicacionId", "NombreAplicacion");
                ViewBag.EmpleadoId = new SelectList(empleadosList, "EmpleadoId", "NombreCompleto");
            }
            else
            {
                // Para Edit (GET) y POST con error - con valor seleccionado
                ViewBag.AplicacionId = new SelectList(aplicaciones, "AplicacionId", "NombreAplicacion", cuentasEmpleado.AplicacionId);
                ViewBag.EmpleadoId = new SelectList(empleadosList, "EmpleadoId", "NombreCompleto", cuentasEmpleado.EmpleadoId);
            }
        }
    }
}