using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGIT.Models;
using SIGIT.ViewModels;

namespace SIGIT.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly SigitContext _context;

        public EmpleadosController(SigitContext context)
        {
            _context = context;
        }

        // GET: Empleados
        public async Task<IActionResult> Index()
        {
            var sigitContext = _context.Empleados.Include(e => e.Area).Include(e => e.Cargo).Include(e => e.Ciudad).Include(e => e.Compania).Include(e => e.Estatus);
            return View(await sigitContext.ToListAsync());
        }

        // Listado de la pagina inicial de empleados
        // GET: Empleados/Listado
        public async Task<IActionResult> Listado()
        {
            var empleadosDesdeBD = await _context.Empleados
                .Include(e => e.Area)
                .Include(e => e.Cargo)
                .Include(e => e.Ciudad)
                .Include(e => e.Estatus)
                .ToListAsync();

            var empleadosParaMostrar = empleadosDesdeBD.Select(e => new EmpleadoListadoViewModel
            {
                Id = e.EmpleadoId,
                NombreCompleto = $"{e.Nombre} {e.SegundoNombre} {e.Apellido} {e.SegundoApellido}".Trim().Replace("  ", " "),
                Celular = e.Celular,
                Area = e.Area.NombreArea,
                Cargo = e.Cargo.NombreCargo,
                Ciudad = e.Ciudad.NombreCiudad,
                Estatus = e.Estatus.NombreEstatus
            }).ToList();

            return View(empleadosParaMostrar);
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .Include(e => e.Area)
                .Include(e => e.Cargo)
                .Include(e => e.Ciudad)
                .Include(e => e.Compania)
                .Include(e => e.Estatus)
                .FirstOrDefaultAsync(m => m.EmpleadoId == id);

            if (empleado == null)
            {
                return NotFound();
            }

            // Devuelve Vista Parcial para el modal
            return PartialView(empleado);
        }

        // GET: Empleados/Create
        public async Task<IActionResult> Create()
        {
            // Llama al método para llenar los dropdowns vacíos
            await PopulateDropdowns();
            // Devuelve Vista Parcial para el modal
            return PartialView();
        }

        // POST: Empleados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpleadoId,Cedula,Nombre,SegundoNombre,Apellido,SegundoApellido,Celular,CorreoPersonal,CargoId,AreaId,CiudadId,CompaniaId,EstatusId,FechaIngreso,FechaRetiro")] Empleado empleado)
        {
            // 1. Asigna la fecha de registro del sistema
            empleado.FechaRegistro = DateTime.Now;

            // 2. Quita el error de FechaRegistro del ModelState
            ModelState.Remove("FechaRegistro");

            // Le decimos al validador que ignore los objetos de navegación nulos,
            // ya que solo nos interesan los IDs (AreaId, CargoId, etc.)
            ModelState.Remove("Area");
            ModelState.Remove("Cargo");
            ModelState.Remove("Ciudad");
            ModelState.Remove("Compania");
            ModelState.Remove("Estatus");
       
            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return Json(new { success = true }); // Devuelve ÉXITO
            }

            // Repoblamos los dropdowns y devolvemos la vista con los errores.
            await PopulateDropdowns(empleado);
            return PartialView(empleado);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            // Llama al método para llenar los dropdowns con los valores actuales del empleado
            await PopulateDropdowns(empleado);
            // Devuelve Vista Parcial para el modal
            return PartialView(empleado);
        }

        // POST: Empleados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmpleadoId,Cedula,Nombre,SegundoNombre,Apellido,SegundoApellido,Celular,CorreoPersonal,CargoId,AreaId,CiudadId,CompaniaId,EstatusId,FechaIngreso,FechaRetiro")] Empleado empleado)
        {
            if (id != empleado.EmpleadoId)
            {
                return NotFound();
            }

            // Quita el error de FechaRegistro del ModelState (no se envía desde el form)
            ModelState.Remove("FechaRegistro");

            // Aplicamos la misma corrección que en el Create
            ModelState.Remove("Area");
            ModelState.Remove("Cargo");
            ModelState.Remove("Ciudad");
            ModelState.Remove("Compania");
            ModelState.Remove("Estatus");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado);

                    // Le decimos a IF que NO MODIFIQUE el campo FechaRegistro
                    _context.Entry(empleado).Property(x => x.FechaRegistro).IsModified = false;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.EmpleadoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Devuelve JSON para indicar éxito al script del modal
                return Json(new { success = true });
            }

            // Si el modelo no es válido, rellena dropdowns y devolver la vista con errores
            await PopulateDropdowns(empleado);
            return PartialView(empleado);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .Include(e => e.Area)
                .Include(e => e.Cargo)
                .Include(e => e.Ciudad)
                .Include(e => e.Compania)
                .Include(e => e.Estatus)
                .FirstOrDefaultAsync(m => m.EmpleadoId == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
            }

            await _context.SaveChangesAsync();
            // Redirige al listado principal (no al Index original)
            return RedirectToAction(nameof(Listado));
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.EmpleadoId == id);
        }

        // Se añade '?' para corregir la advertencia de nulos
        private async Task PopulateDropdowns(Empleado? empleado = null)
        {
            if (empleado == null)
            {
                // Para Create (GET) - sin selección
                ViewBag.AreaId = new SelectList(await _context.Areas.ToListAsync(), "AreaId", "NombreArea");
                ViewBag.CargoId = new SelectList(await _context.Cargos.ToListAsync(), "CargoId", "NombreCargo");
                ViewBag.CiudadId = new SelectList(await _context.Ciudades.ToListAsync(), "CiudadId", "NombreCiudad");
                ViewBag.CompaniaId = new SelectList(await _context.Companias.ToListAsync(), "CompaniaId", "NombreCompania");
                ViewBag.EstatusId = new SelectList(await _context.Estatuses.ToListAsync(), "EstatusId", "NombreEstatus");
            }
            else
            {
                // Para Edit (GET) y POST con error - con valor seleccionado
                ViewBag.AreaId = new SelectList(await _context.Areas.ToListAsync(), "AreaId", "NombreArea", empleado.AreaId);
                ViewBag.CargoId = new SelectList(await _context.Cargos.ToListAsync(), "CargoId", "NombreCargo", empleado.CargoId);
                ViewBag.CiudadId = new SelectList(await _context.Ciudades.ToListAsync(), "CiudadId", "NombreCiudad", empleado.CiudadId);
                ViewBag.CompaniaId = new SelectList(await _context.Companias.ToListAsync(), "CompaniaId", "NombreCompania", empleado.CompaniaId);
                ViewBag.EstatusId = new SelectList(await _context.Estatuses.ToListAsync(), "EstatusId", "NombreEstatus", empleado.EstatusId);
            }
        }
    }
}