 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace peluqueria_barberia.API.Models
{
    [Table("Roles")]
    public class Rol
    {
        [Key]
        public int RolID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        // Relación con Usuarios
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}