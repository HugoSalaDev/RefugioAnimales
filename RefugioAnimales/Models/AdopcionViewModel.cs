using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RefugioAnimales.Models.ViewModels
{
    public class AdopcionViewModel
    {
        // Datos del Animal (solo lectura para mostrar)
        public int AnimalId { get; set; }
        public string AnimalNombre { get; set; } = string.Empty;

        // Datos del Formulario (lo que el usuario rellena)
        [Required(ErrorMessage = "Debes seleccionar un adoptante.")]
        public int? AdoptanteId { get; set; }

        [Required(ErrorMessage = "La fecha de adopción es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaAdopcion { get; set; } = DateTime.Now;

        // La lista para el desplegable (Select)
        // Al estar aquí tipada, nos olvidamos del ViewBag
        public IEnumerable<SelectListItem>? ListaAdoptantes { get; set; }
    }
}