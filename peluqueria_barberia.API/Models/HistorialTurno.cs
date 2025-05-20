using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace peluqueria_barberia.API.Models
{
    [Table("HistorialTurnos")]
    public class HistorialTurno
    {
        [Key]
        public int HistorialID { get; set; }

        [Required]
        public int TurnoID { get; set; }

        [Required]
        public int UsuarioID { get; set; }

        public DateTime FechaCambio { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Accion { get; set; }

        [StringLength(255)]
        public string Comentario { get; set; }

        // Relaciones
        [ForeignKey("TurnoID")]
        public virtual Turno Turno { get; set; }

        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }
    }
} 