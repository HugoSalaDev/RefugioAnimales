using Microsoft.AspNetCore.Mvc;
using RefugioAnimales.Data;
using RefugioAnimales.Helpers;
using Microsoft.EntityFrameworkCore;

namespace RefugioAnimales.Controllers
{
    public class AccountController : Controller
    {
        private readonly RefugioContext _context;

        public AccountController(RefugioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string password)
        {
            // Buscar usuario por nombre
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            if (usuario != null)
            {
                // Verificar contraseña usando el mismo algoritmo que el Seed
                var hashEntrada = Seguridad.HashPassword(password, usuario.Salt);

                if (hashEntrada == usuario.PasswordHash)
                {
                    // Login exitoso: Guardar en sesión
                    HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                    HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
                    HttpContext.Session.SetString("Rol", usuario.Rol);

                    return RedirectToAction("Inicio", "Refugio");
                }
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Inicio", "Refugio");
        }
    }
}