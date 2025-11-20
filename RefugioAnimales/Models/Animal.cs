using System.ComponentModel.DataAnnotations;

namespace RefugioAnimales.Models
{
    // Modelo Animal con validaciones y soporte para imágenes en BD
    public class Animal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especie es obligatoria")]
        [StringLength(30, ErrorMessage = "La especie no puede superar 30 caracteres")]
        public string Especie { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es obligatoria")]
        [Range(0, 30, ErrorMessage = "La edad debe estar entre 0 y 30 años")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20, ErrorMessage = "El estado no puede superar 20 caracteres")]
        public string Estado { get; set; } = "Disponible";

        [StringLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres")]
        public string? Descripcion { get; set; }

        // Imagen guardada en la base de datos como byte array
        public byte[]? FotoContenido { get; set; }

        // Tipo MIME de la imagen (image/jpeg, image/png, etc.)
        public string? FotoMimeType { get; set; }

        // Relación con el adoptante (nullable porque puede no estar adoptado)
        public int? AdoptanteId { get; set; }
        public Adoptante? Adoptante { get; set; }

        // Fecha de adopción
        public DateTime? FechaAdopcion { get; set; }
    }
}