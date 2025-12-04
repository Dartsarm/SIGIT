using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGIT.Models;

using Newtonsoft.Json; 

namespace SIGIT.Controllers
{
    public class CargoesController : Controller
    {
        private readonly SigitContext _context;

        public CargoesController(SigitContext context)
        {
            _context = context;
        }

        // GET: Cargoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cargos.ToListAsync());
        }

        // GET: Cargoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos
                .FirstOrDefaultAsync(m => m.CargoId == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return PartialView(cargo);
        }

        // GET: Cargoes/Create
        public IActionResult Create()
        {
            return PartialView();
        }

        // POST: Cargoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CargoId,NombreCargo")] Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cargo);
                await _context.SaveChangesAsync();
                // CAMBIO CLAVE AQUÍ: En lugar de RedirectToAction, devuelve un indicador de éxito para AJAX
                return Json(new { success = true }); // Indica que la operación fue exitosa, ojo se debe tener en cuenta que devuelva el json en el return para que todo funcione bien
            }
            // Si hay errores de validación, devuelve la vista parcial para que los errores se muestren en el modal
            return PartialView(cargo);
        }

        // GET: Cargoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo == null)
            {
                return NotFound();
            }
            return PartialView(cargo);
        }

        // POST: Cargoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CargoId,NombreCargo")] Cargo cargo)
        {
            if (id != cargo.CargoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cargo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CargoExists(cargo.CargoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // CAMBIO CLAVE AQUÍ: En lugar de RedirectToAction, devuelve un indicador de éxito para AJAX
                return Json(new { success = true }); // Indica que la operación fue exitosa, ojo se debe tener en cuenta que devuelva el json en el return para que todo funcione bien
            }
            // Si hay errores de validación, devuelve la vista parcial para que los errores se muestren en el modal
            return PartialView(cargo);
        }

        // GET: Cargoes/Delete/5
        // Este método sigue siendo una vista de página completa, lo dejaremos como está
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos
                .FirstOrDefaultAsync(m => m.CargoId == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo); // Devuelve la vista completa para la confirmación de eliminación
        }

        // POST: Cargoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo != null)
            {
                _context.Cargos.Remove(cargo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CargoExists(int id)
        {
            return _context.Cargos.Any(e => e.CargoId == id);
        }
    }
}