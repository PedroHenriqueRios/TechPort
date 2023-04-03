using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using TechPort.Data;
using TechPort.Models;

namespace TechPort.Controllers
{
    public class NaviosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NaviosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Navios
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1; // se o parâmetro page for nulo, use a primeira página como padrão
            var pageSize = 3; // defina o número de registros exibidos em cada página

            
            var navios = await _context.Navios.Include(n => n.Empresa)
                                                 .ToPagedListAsync(pageNumber, pageSize) as IPagedList<Navio>; ;
            return View(navios);
        }

        [Authorize]
        // GET: Navios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Navios == null)
            {
                return NotFound();
            }

            var navio = await _context.Navios
                .Include(n => n.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (navio == null)
            {
                return NotFound();
            }

            return View(navio);
        }

        [Authorize]
        // GET: Navios/Create
        public IActionResult Create()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Nome");
            return View();
        }

        [Authorize]
        // POST: Navios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,EmpresaId")] Navio navio)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                _context.Add(navio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Nome", navio.EmpresaId);
            return View(navio);
        }

        [Authorize]
        // GET: Navios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Navios == null)
            {
                return NotFound();
            }

            var navio = await _context.Navios.FindAsync(id);
            if (navio == null)
            {
                return NotFound();
            }
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Nome", navio.EmpresaId);
            return View(navio);
        }

        [Authorize]
        // POST: Navios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,EmpresaId")] Navio navio)
        {
            if (id != navio.Id)
            {
                return NotFound();
            }

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(navio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NavioExists(navio.Id))
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
            ViewData["EmpresaId"] = new SelectList(_context.Empresa, "Id", "Nome", navio.EmpresaId);
            return View(navio);
        }

        [Authorize]
        // GET: Navios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Navios == null)
            {
                return NotFound();
            }

            var navio = await _context.Navios
                .Include(n => n.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (navio == null)
            {
                return NotFound();
            }

            return View(navio);
        }

        [Authorize]
        // POST: Navios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Navios == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Navios'  is null.");
            }
            var navio = await _context.Navios.FindAsync(id);
            if (navio != null)
            {
                _context.Navios.Remove(navio);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NavioExists(int id)
        {
          return _context.Navios.Any(e => e.Id == id);
        }
    }
}
