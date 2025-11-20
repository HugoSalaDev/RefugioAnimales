using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefugioAnimales.Data;
using RefugioAnimales.Models;

namespace RefugioAnimales.Controllers
{
    public class AdoptantesController : Controller
    {
        private readonly RefugioContext _context;

        public AdoptantesController(RefugioContext context)
        {
            _context = context;
        }

        // LISTAR
        public async Task<IActionResult> Index()
        {
            return View(await _context.Adoptantes.ToListAsync());
        }

        // DETALLE
        public async Task<IActionResult> Detalle(int? id)
        {
            if (id == null) return NotFound();

            var adoptante = await _context.Adoptantes
                .Include(a => a.Animales) // Incluir animales adoptados para verlos en el detalle
                .FirstOrDefaultAsync(m => m.Id == id);

            if (adoptante == null) return NotFound();

            return View(adoptante);
        }

        // CREAR - GET
        public IActionResult Crear()
        {
            return View();
        }

        // CREAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Adoptante adoptante)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adoptante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adoptante);
        }

        // EDITAR - GET
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();
            var adoptante = await _context.Adoptantes.FindAsync(id);
            if (adoptante == null) return NotFound();
            return View(adoptante);
        }

        // EDITAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Adoptante adoptante)
        {
            if (id != adoptante.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adoptante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Adoptantes.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(adoptante);
        }

        // ELIMINAR - GET
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null) return NotFound();
            var adoptante = await _context.Adoptantes.FirstOrDefaultAsync(m => m.Id == id);
            if (adoptante == null) return NotFound();
            return View(adoptante);
        }

        // ELIMINAR - POST
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var adoptante = await _context.Adoptantes.FindAsync(id);
            if (adoptante != null)
            {
                _context.Adoptantes.Remove(adoptante);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}