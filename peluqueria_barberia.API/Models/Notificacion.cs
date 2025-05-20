using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace peluqueria_barberia.API.Models
{
    [Table("Notificaciones")]
    public class Notificacion
    {
        [Key]
        public int NotificacionID { get; set; }

        [Required]
        public int TurnoID { get; set; }

        [StringLength(50)]
        public string Tipo { get; set; }

        [StringLength(20)]
        public string Estado { get; set; }

        public DateTime? FechaEnvio { get; set; }

        // Relación
        [ForeignKey("TurnoID")]
        public virtual Turno Turno { get; set; }
    }
} 