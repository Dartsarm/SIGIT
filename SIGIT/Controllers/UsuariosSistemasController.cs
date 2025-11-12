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

            return View(usuariosSistema);
        }

        // GET: UsuariosSistemas/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "RolId");
            return View();
        }

        // POST: UsuariosSistemas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioSistemaId,Cedula,Nombre,SegundoNombre,Apellido,SegundoApellido,UsuarioLogin,Email,Celular,PasswordHash,RolId,Activo,FechaRegistro")] UsuariosSistema usuariosSistema)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuariosSistema);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "RolId", usuariosSistema.RolId);
            return View(usuariosSistema);
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
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "RolId", usuariosSistema.RolId);
            return View(usuariosSistema);
        }

        // POST: UsuariosSistemas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioSistemaId,Cedula,Nombre,SegundoNombre,Apellido,SegundoApellido,UsuarioLogin,Email,Celular,PasswordHash,RolId,Activo,FechaRegistro")] UsuariosSistema usuariosSistema)
        {
            if (id != usuariosSistema.UsuarioSistemaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuariosSistema);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "RolId", usuariosSistema.RolId);
            return View(usuariosSistema);
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
    }
}
