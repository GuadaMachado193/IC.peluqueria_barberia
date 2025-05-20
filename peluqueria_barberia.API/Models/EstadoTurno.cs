using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace peluqueria_barberia.API.Models
{
    public class EstadoTurno
    {
        [Key]
        public int EstadoID { get; set; }

        [Required]
        [StringLength(20)]
        public string Nombre { get; set; }

        // Relaciones
        public virtual ICollection<Turno> Turnos { get; set; }
    }
} 