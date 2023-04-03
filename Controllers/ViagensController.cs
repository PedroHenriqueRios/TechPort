using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using TechPort.Data;
using TechPort.Models;

namespace TechPort.Controllers
{
    public class ViagensController : Controller
    {
        private readonly ApplicationDbContext _context;
        private int? page;

        public ViagensController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Viagens
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1; // se o parâmetro page for nulo, use a primeira página como padrão
            var pageSize = 3; // defina o número de registros exibidos em cada página

            var viagens = await _context.Viagens.Include(v => v.Navio)
                                                 .ToPagedListAsync(pageNumber, pageSize) as IPagedList<Viagem>; ;
            return View(viagens);
        }

        [Authorize]
        // GET: Viagens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Viagens == null)
            {
                return NotFound();
            }

            var viagem = await _context.Viagens
                .Include(v => v.Navio)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viagem == null)
            {
                return NotFound();
            }

            return View(viagem);
        }

        [Authorize]
        // GET: Viagens/Create
        public IActionResult Create()
        {
            ViewData["NavioId"] = new SelectList(_context.Navios, "Id", "Nome");
            return View();
        }

        [Authorize]
        // POST: Viagens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codigo,NavioId,TipoViagem")] Viagem viagem)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                _context.Add(viagem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NavioId"] = new SelectList(_context.Navios, "Id", "Nome", viagem.NavioId);
            return View(viagem);
        }

        [Authorize]
        // GET: Viagens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Viagens == null)
            {
                return NotFound();
            }

            var viagem = await _context.Viagens.FindAsync(id);
            if (viagem == null)
            {
                return NotFound();
            }
            ViewData["NavioId"] = new SelectList(_context.Navios, "Id", "Nome", viagem.NavioId);
            return View(viagem);
        }

        [Authorize]
        // POST: Viagens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,NavioId,TipoViagem")] Viagem viagem)
        {
            if (id != viagem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viagem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViagemExists(viagem.Id))
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
            ViewData["NavioId"] = new SelectList(_context.Navios, "Id", "Nome", viagem.NavioId);
            return View(viagem);
        }

        [Authorize]
        // GET: Viagens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Viagens == null)
            {
                return NotFound();
            }

            var viagem = await _context.Viagens
                .Include(v => v.Navio)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viagem == null)
            {
                return NotFound();
            }

            return View(viagem);
        }

        [Authorize]
        // POST: Viagens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Viagens == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Viagens'  is null.");
            }
            var viagem = await _context.Viagens.FindAsync(id);
            if (viagem != null)
            {
                _context.Viagens.Remove(viagem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViagemExists(int id)
        {
          return _context.Viagens.Any(e => e.Id == id);
        }
    }
}
