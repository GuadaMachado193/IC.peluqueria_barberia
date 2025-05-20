using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace peluqueria_barberia.API.Models
{
    [Table("Turnos")]
    public class Turno
    {
        [Key]
        public int TurnoID { get; set; }

        [Required]
        public int ClienteID { get; set; }

        [Required]
        public int EmpleadoID { get; set; }

        [Required]
        public int ServicioID { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public string Notas { get; set; }

        [Required]
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relaciones
        [ForeignKey("ClienteID")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("EmpleadoID")]
        public virtual Empleado Empleado { get; set; }

        [ForeignKey("ServicioID")]
        public virtual Servicio Servicio { get; set; }

        public virtual ICollection<HistorialTurno> HistorialTurnos { get; set; }
        public virtual ICollection<Notificacion> Notificaciones { get; set; }
    }
} 