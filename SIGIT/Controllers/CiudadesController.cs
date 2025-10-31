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
    public class CiudadesController : Controller
    {
        private readonly SigitContext _context;

        public CiudadesController(SigitContext context)
        {
            _context = context;
        }

        // GET: Ciudades
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ciudades.ToListAsync());
        }

        // GET: Ciudades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ciudade = await _context.Ciudades
                .FirstOrDefaultAsync(m => m.CiudadId == id);
            if (ciudade == null)
            {
                return NotFound();
            }

            return PartialView(ciudade);
        }

        // GET: Ciudades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ciudades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CiudadId,NombreCiudad")] Ciudade ciudade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ciudade);
                await _context.SaveChangesAsync();
                return Json(new { success = true }); /*RedirectToAction(nameof(Index));*/
            }
            return PartialView(ciudade);
        }

        // GET: Ciudades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ciudade = await _context.Ciudades.FindAsync(id);
            if (ciudade == null)
            {
                return NotFound();
            }
            return PartialView(ciudade);
        }

        // POST: Ciudades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CiudadId,NombreCiudad")] Ciudade ciudade)
        {
            if (id != ciudade.CiudadId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ciudade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CiudadeExists(ciudade.CiudadId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new { success = true }); /*RedirectToAction(nameof(Index));*/
            }
            return PartialView(ciudade);
        }

        // GET: Ciudades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ciudade = await _context.Ciudades
                .FirstOrDefaultAsync(m => m.CiudadId == id);
            if (ciudade == null)
            {
                return NotFound();
            }

            return View(ciudade);
        }

        // POST: Ciudades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ciudade = await _context.Ciudades.FindAsync(id);
            if (ciudade != null)
            {
                _context.Ciudades.Remove(ciudade);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CiudadeExists(int id)
        {
            return _context.Ciudades.Any(e => e.CiudadId == id);
        }
    }
}
