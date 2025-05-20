using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace peluqueria_barberia.API.Models
{
    [Table("EmpleadoServicio")]
    public class EmpleadoServicio
    {
        [Key]
        [Column(Order = 0)]
        public int EmpleadoID { get; set; }

        [Key]
        [Column(Order = 1)]
        public int ServicioID { get; set; }

        [ForeignKey("EmpleadoID")]
        public virtual Empleado Empleado { get; set; }

        [ForeignKey("ServicioID")]
        public virtual Servicio Servicio { get; set; }
    }
} 