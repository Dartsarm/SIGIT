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
            if (id == null)
            {
                return NotFound();
            }

            var aplicacione = await _context.Aplicaciones
                .FirstOrDefaultAsync(m => m.AplicacionId == id);
            if (aplicacione == null)
            {
                return NotFound();
            }

            return View(aplicacione);
        }

        // GET: Aplicaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aplicaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AplicacionId,NombreAplicacion")] Aplicacione aplicacione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aplicacione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aplicacione);
        }

        // GET: Aplicaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aplicacione = await _context.Aplicaciones.FindAsync(id);
            if (aplicacione == null)
            {
                return NotFound();
            }
            return View(aplicacione);
        }

        // POST: Aplicaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AplicacionId,NombreAplicacion")] Aplicacione aplicacione)
        {
            if (id != aplicacione.AplicacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aplicacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AplicacioneExists(aplicacione.AplicacionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aplicacione);
        }

        // GET: Aplicaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aplicacione = await _context.Aplicaciones
                .FirstOrDefaultAsync(m => m.AplicacionId == id);
            if (aplicacione == null)
            {
                return NotFound();
            }

            return View(aplicacione);
        }

        // POST: Aplicaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aplicacione = await _context.Aplicaciones.FindAsync(id);
            if (aplicacione != null)
            {
                _context.Aplicaciones.Remove(aplicacione);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AplicacioneExists(int id)
        {
            return _context.Aplicaciones.Any(e => e.AplicacionId == id);
        }
    }
}
