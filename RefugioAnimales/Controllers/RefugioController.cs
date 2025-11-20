using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RefugioAnimales.Data;
using RefugioAnimales.Models;
using RefugioAnimales.Models.ViewModels;
using System.IO;

namespace RefugioAnimales.Controllers
{
    // Controlador principal del refugio
    // Gestiona todas las operaciones CRUD de animales
    public class RefugioController : Controller
    {
        private readonly RefugioContext _context;

        // Inyección de dependencias - recibo el contexto de BD
        public RefugioController(RefugioContext context)
        {
            _context = context;
        }

        // ========================================
        // PÁGINA DE INICIO
        // ========================================
        public IActionResult Inicio()
        {
            return View();
        }

        // ========================================
        // LISTAR TODOS LOS ANIMALES
        // ========================================
        public async Task<IActionResult> Animales()
        {
            // Obtengo todos los animales de la BD
            var animales = await _context.Animales
                .Include(a => a.Adoptante) // Incluyo info del adoptante si existe
                .ToListAsync();
            return View(animales);
        }

        // ========================================
        // VER DETALLE DE UN ANIMAL
        // ========================================
        public async Task<IActionResult> Detalle(int id)
        {
            // Busco el animal por id, incluyendo su adoptante
            var animal = await _context.Animales
                .Include(a => a.Adoptante)
                .FirstOrDefaultAsync(a => a.Id == id);

            // Si no existe, redirijo al listado
            if (animal == null)
            {
                return RedirectToAction("Animales");
            }

            return View(animal);
        }

        // ========================================
        // CREAR ANIMAL - GET (mostrar formulario)
        // ========================================
        [HttpGet]
        public IActionResult Crear()
        {
            // Comprobar si hay sesión
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // ========================================
        // CREAR ANIMAL - POST (guardar en BD)
        // ========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)] // Límite de 100 MB
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<IActionResult> Crear(Animal animal, IFormFile? foto)
        {
            // Comprobar si hay sesión
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Quito la validación de adoptante y fechas porque son opcionales
            ModelState.Remove("Adoptante");
            ModelState.Remove("FechaAdopcion");
            ModelState.Remove("AdoptanteId");

            if (ModelState.IsValid)
            {
                // Si se subió una foto, la proceso
                if (foto != null && foto.Length > 0)
                {
                    try
                    {
                        // Log para debug
                        Console.WriteLine($"Procesando imagen: {foto.FileName}, Tamaño: {foto.Length} bytes");

                        // Validar tamaño máximo (10 MB)
                        if (foto.Length > 10 * 1024 * 1024)
                        {
                            ModelState.AddModelError("foto", "La imagen no puede superar los 10 MB");
                            return View(animal);
                        }

                        // Validar que sea una imagen
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

                        if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                        {
                            ModelState.AddModelError("foto", "Solo se permiten imágenes (JPG, PNG, GIF, BMP)");
                            return View(animal);
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            await foto.CopyToAsync(memoryStream);
                            animal.FotoContenido = memoryStream.ToArray();
                            animal.FotoMimeType = foto.ContentType;
                            Console.WriteLine($"Imagen procesada correctamente: {animal.FotoContenido.Length} bytes");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR al procesar imagen: {ex.Message}");
                        Console.WriteLine($"StackTrace: {ex.StackTrace}");
                        ModelState.AddModelError("foto", $"Error al procesar la imagen: {ex.Message}");
                        return View(animal);
                    }
                }

                try
                {
                    Console.WriteLine("Guardando animal en la BD...");

                    // Guardo el animal en la BD
                    _context.Animales.Add(animal);
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Animal guardado correctamente");

                    // Redirijo al listado
                    return RedirectToAction("Animales");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR al guardar en BD: {ex.Message}");
                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                    return View(animal);
                }
            }

            // Si hay errores de validación, vuelvo a mostrar el formulario
            Console.WriteLine("Errores de validación:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"  - {error.ErrorMessage}");
            }

            return View(animal);
        }

        // ========================================
        // EDITAR ANIMAL - GET (mostrar formulario)
        // ========================================
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            // Comprobar si hay sesión
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var animal = await _context.Animales.FindAsync(id);

            if (animal == null)
            {
                return RedirectToAction("Animales");
            }

            return View(animal);
        }

        // ========================================
        // EDITAR ANIMAL - POST (actualizar en BD)
        // ========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Animal animal, IFormFile? foto)
        {
            // Comprobar si hay sesión
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != animal.Id)
            {
                return RedirectToAction("Animales");
            }

            // Quito validaciones opcionales
            ModelState.Remove("Adoptante");
            ModelState.Remove("FechaAdopcion");
            ModelState.Remove("AdoptanteId");

            if (ModelState.IsValid)
            {
                try
                {
                    // Si se subió una nueva foto, la actualizo
                    if (foto != null && foto.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await foto.CopyToAsync(memoryStream);
                            animal.FotoContenido = memoryStream.ToArray();
                            animal.FotoMimeType = foto.ContentType;
                        }
                    }
                    else
                    {
                        // Si no se subió foto nueva, mantengo la anterior
                        var animalExistente = await _context.Animales.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                        if (animalExistente != null)
                        {
                            animal.FotoContenido = animalExistente.FotoContenido;
                            animal.FotoMimeType = animalExistente.FotoMimeType;
                        }
                    }

                    // Actualizo en la BD
                    _context.Update(animal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Si hubo un error de concurrencia, verifico si el animal existe
                    if (!await AnimalExiste(animal.Id))
                    {
                        return RedirectToAction("Animales");
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Animales");
            }

            return View(animal);
        }

        // ========================================
        // ELIMINAR ANIMAL - GET (confirmación)
        // ========================================
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Comprobar si hay sesión
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var animal = await _context.Animales
                .Include(a => a.Adoptante)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
            {
                return RedirectToAction("Animales");
            }

            return View(animal);
        }

        // ========================================
        // ELIMINAR ANIMAL - POST (borrar de BD)
        // ========================================
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            // Comprobar si hay sesión
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var animal = await _context.Animales.FindAsync(id);

            if (animal != null)
            {
                _context.Animales.Remove(animal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Animales");
        }

        // ========================================
        // OBTENER IMAGEN DE UN ANIMAL
        // ========================================
        public async Task<IActionResult> ObtenerImagen(int id)
        {
            var animal = await _context.Animales.FindAsync(id);

            // Si no tiene foto, devuelvo error 404
            if (animal == null || animal.FotoContenido == null)
            {
                return NotFound();
            }

            // Devuelvo la imagen con su tipo MIME
            return File(animal.FotoContenido, animal.FotoMimeType ?? "image/jpeg");
        }

        // ========================================
        // MÉTODO AUXILIAR: Verificar si animal existe
        // ========================================
        private async Task<bool> AnimalExiste(int id)
        {
            return await _context.Animales.AnyAsync(e => e.Id == id);
        }

        // ========================================
        // ADOPTAR - GET
        // ========================================
        [HttpGet]
        public async Task<IActionResult> Adoptar(int id)
        {
            // Comprobar si hay sesión
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var animal = await _context.Animales.FindAsync(id);
            if (animal == null) return NotFound();

            if (animal.Estado == "Adoptado")
            {
                return RedirectToAction("Detalle", new { id = animal.Id });
            }

            // Cear el ViewModel con los datos necesarios
            var viewModel = new AdopcionViewModel
            {
                AnimalId = animal.Id,
                AnimalNombre = animal.Nombre,
                FechaAdopcion = DateTime.Now,
                // Cargamos la lista aquí, tipada fuertemente
                ListaAdoptantes = await _context.Adoptantes
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Nombre
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // ========================================
        // ADOPTAR - POST (Recibe el ViewModel)
        // ========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adoptar(AdopcionViewModel model)
        {
            // Comprobar si hay sesión
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var animal = await _context.Animales.FindAsync(model.AnimalId);
                if (animal == null) return NotFound();

                // Actualizamos la entidad Animal con los datos del ViewModel
                animal.AdoptanteId = model.AdoptanteId;
                animal.FechaAdopcion = model.FechaAdopcion;
                animal.Estado = "Adoptado";

                _context.Update(animal);
                await _context.SaveChangesAsync();

                return RedirectToAction("Detalle", new { id = animal.Id });
            }

            // Si hay error (ej: no seleccionó adoptante), recargamos la lista
            // porque el protocolo HTTP no guarda el estado de la lista
            model.ListaAdoptantes = await _context.Adoptantes
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nombre
                })
                .ToListAsync();

            return View(model);
        }
        // ========================================
        // DESADOPTAR (Liberar animal)
        // ========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desadoptar(int id)
        {
            // SEGURIDAD: Solo usuarios logueados
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var animal = await _context.Animales.FindAsync(id);
            if (animal == null) return NotFound();

            // Restablecer valores para liberar al animal
            animal.AdoptanteId = null;
            animal.FechaAdopcion = null;
            animal.Estado = "Disponible";

            _context.Update(animal);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detalle", new { id = animal.Id });
        }
    }
}