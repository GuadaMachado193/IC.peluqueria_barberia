using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace peluqueria_barberia.API.Models
{
    [Table("Servicios")]
    public class Servicio
    {
        [Key]
        public int ServicioID { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int DuracionMinutos { get; set; }

        [StringLength(20)]
        public string Estado { get; set; }

        // Relaciones
        public virtual ICollection<EmpleadoServicio> EmpleadoServicios { get; set; }
        public virtual ICollection<Turno> Turnos { get; set; }
    }
} 