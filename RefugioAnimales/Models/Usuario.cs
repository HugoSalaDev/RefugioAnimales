using System.ComponentModel.DataAnnotations;

namespace RefugioAnimales.Models
{
    // Modelo para usuarios del sistema (login)
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede superar 50 caracteres")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Salt { get; set; } = string.Empty;

        [Required]
        [StringLength(20, ErrorMessage = "El rol no puede superar 20 caracteres")]
        public string Rol { get; set; } = "Usuario"; // "Admin" o "Usuario"
    }
}