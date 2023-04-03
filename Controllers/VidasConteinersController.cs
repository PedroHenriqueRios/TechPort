using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using TechPort.Data;
using TechPort.Models;

namespace TechPort.Controllers
{
    public class VidasConteinersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VidasConteinersController(ApplicationDbContext context)
        {
            _context = context;

        }

        [Authorize]
        // GET: VidasConteiners
        public async Task<IActionResult> Index(int? page)
        {

            var pageNumber = page ?? 1; // se o parâmetro page for nulo, use a primeira página como padrão
            var pageSize = 3; // defina o número de registros exibidos em cada página

            var vidaConteiners = await _context.VidasConteiner.Include(v => v.Conteiner).Include(v => v.Viagem)
                                                 .ToPagedListAsync(pageNumber, pageSize) as IPagedList<VidaConteiner>;
            return View(vidaConteiners);
        }

        [Authorize]
        // GET: VidasConteiners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.VidasConteiner == null)
            {
                return NotFound();
            }

            var vidaConteiner = await _context.VidasConteiner
                .Include(v => v.Conteiner)
                .Include(v => v.Viagem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vidaConteiner == null)
            {
                return NotFound();
            }

            return View(vidaConteiner);
        }

        [Authorize]
        // GET: VidasConteiners/Create
        public IActionResult Create()
        {
            ViewData["ConteinerId"] = new SelectList(_context.Conteiners, "Id", "Nome");
            ViewData["ViagemId"] = new SelectList(_context.Viagens, "Id", "Codigo");
            return View();
        }

        [Authorize]
        // POST: VidasConteiners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ConteinerId,StatusConteiner,ViagemId,Navio,Etapa")] VidaConteiner vidaConteiner)
        {
            #region Inclui Nome do Navio Diretamente na Coluna
            var NavioContext = _context.Viagens.Include(v => v.Navio);
            vidaConteiner.Navio = NavioContext.Where(v => v.Id == vidaConteiner.ViagemId).Select(v => v.Navio.Nome).FirstOrDefault();        
            #endregion


            ModelState.Clear();

            if (ModelState.IsValid)
            {
                _context.Add(vidaConteiner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConteinerId"] = new SelectList(_context.Conteiners, "Id", "Nome", vidaConteiner.ConteinerId);
            ViewData["ViagemId"] = new SelectList(_context.Viagens, "Id", "Codigo", vidaConteiner.ViagemId);
            ViewData["NavioId"] = new SelectList(_context.Navios, "Id", "Nome", vidaConteiner.Viagem.NavioId);
            return View(vidaConteiner);
        }

        [Authorize]
        // GET: VidasConteiners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.VidasConteiner == null)
            {
                return NotFound();
            }

            var vidaConteiner = await _context.VidasConteiner.FindAsync(id);
            if (vidaConteiner == null)
            {
                return NotFound();
            }
            ViewData["ConteinerId"] = new SelectList(_context.Conteiners, "Id", "Nome", vidaConteiner.ConteinerId);
            ViewData["ViagemId"] = new SelectList(_context.Viagens, "Id", "Codigo", vidaConteiner.ViagemId);
            return View(vidaConteiner);
        }

        [Authorize]
        // POST: VidasConteiners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConteinerId,StatusConteiner,ViagemId,Navio,Etapa")] VidaConteiner vidaConteiner)
        {
            if (id != vidaConteiner.Id)
            {
                return NotFound();
            }

            #region Inclui Nome do Navio Diretamente na Coluna
            var NavioContext = _context.Viagens.Include(v => v.Navio);
            vidaConteiner.Navio = NavioContext.Where(v => v.Id == vidaConteiner.ViagemId).Select(v => v.Navio.Nome).FirstOrDefault();
            #endregion

            ModelState.Clear();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vidaConteiner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VidaConteinerExists(vidaConteiner.Id))
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
            ViewData["ConteinerId"] = new SelectList(_context.Conteiners, "Id", "Nome", vidaConteiner.ConteinerId);
            ViewData["ViagemId"] = new SelectList(_context.Viagens, "Id", "Codigo", vidaConteiner.ViagemId);
            return View(vidaConteiner);
        }

        [Authorize]
        // GET: VidasConteiners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.VidasConteiner == null)
            {
                return NotFound();
            }

            var vidaConteiner = await _context.VidasConteiner
                .Include(v => v.Conteiner)
                .Include(v => v.Viagem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vidaConteiner == null)
            {
                return NotFound();
            }

            return View(vidaConteiner);
        }

        [Authorize]
        // POST: VidasConteiners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.VidasConteiner == null)
            {
                return Problem("Entity set 'ApplicationDbContext.VidasConteiner'  is null.");
            }
            var vidaConteiner = await _context.VidasConteiner.FindAsync(id);
            if (vidaConteiner != null)
            {
                _context.VidasConteiner.Remove(vidaConteiner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VidaConteinerExists(int id)
        {
            return _context.VidasConteiner.Any(e => e.Id == id);
        }
    }
}
