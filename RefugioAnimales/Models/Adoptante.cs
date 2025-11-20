using System.ComponentModel.DataAnnotations;

namespace RefugioAnimales.Models
{
    // Modelo para personas que adoptan animales
    public class Adoptante
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede superar 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(15, ErrorMessage = "El teléfono no puede superar 15 caracteres")]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        public DateTime FechaAlta { get; set; } = DateTime.Now;

        // Relación: un adoptante puede tener múltiples animales adoptados
        public List<Animal> Animales { get; set; } = new List<Animal>();
    }
}