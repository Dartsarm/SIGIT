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
    public class EstatusController : Controller
    {
        private readonly SigitContext _context;

        public EstatusController(SigitContext context)
        {
            _context = context;
        }

        // GET: Estatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.Estatuses.ToListAsync());
        }

        // GET: Estatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatus = await _context.Estatuses
                .FirstOrDefaultAsync(m => m.EstatusId == id);
            if (estatus == null)
            {
                return NotFound();
            }

            return PartialView(estatus);
        }

        // GET: Estatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EstatusId,NombreEstatus")] Estatus estatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estatus);
                await _context.SaveChangesAsync();
                return Json(new { success = true }); /*RedirectToAction(nameof(Index));*/
            }
            return PartialView(estatus);
        }

        // GET: Estatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatus = await _context.Estatuses.FindAsync(id);
            if (estatus == null)
            {
                return NotFound();
            }
            return PartialView(estatus);
        }

        // POST: Estatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstatusId,NombreEstatus")] Estatus estatus)
        {
            if (id != estatus.EstatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstatusExists(estatus.EstatusId))
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
            return PartialView(estatus);
        }

        // GET: Estatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatus = await _context.Estatuses
                .FirstOrDefaultAsync(m => m.EstatusId == id);
            if (estatus == null)
            {
                return NotFound();
            }

            return View(estatus);
        }

        // POST: Estatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estatus = await _context.Estatuses.FindAsync(id);
            if (estatus != null)
            {
                _context.Estatuses.Remove(estatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstatusExists(int id)
        {
            return _context.Estatuses.Any(e => e.EstatusId == id);
        }
    }
}
