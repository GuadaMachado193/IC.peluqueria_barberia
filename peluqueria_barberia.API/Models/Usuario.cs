using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace peluqueria_barberia.API.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(100)]
        public string Usuario { get; set; }

        [Required]
        [StringLength(255)]
        public string Clave { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        public int RolID { get; set; }

        [ForeignKey("RolID")]
        public virtual Rol Rol { get; set; }
    }
} 