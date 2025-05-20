using System.Data.Entity;
using peluqueria_barberia.API.Models;

namespace peluqueria_barberia.API.Data
{
    public class BarberiaEsteticaContext : DbContext
    {
        public BarberiaEsteticaContext() : base("name=BarberiaEsteticaConnection")
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<EmpleadoServicio> EmpleadoServicios { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<HistorialTurno> HistorialTurnos { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configuración de la relación muchos a muchos entre Empleado y Servicio
            modelBuilder.Entity<EmpleadoServicio>()
                .HasKey(es => new { es.EmpleadoID, es.ServicioID });

            modelBuilder.Entity<EmpleadoServicio>()
                .HasRequired(es => es.Empleado)
                .WithMany(e => e.EmpleadoServicios)
                .HasForeignKey(es => es.EmpleadoID);

            modelBuilder.Entity<EmpleadoServicio>()
                .HasRequired(es => es.Servicio)
                .WithMany(s => s.EmpleadoServicios)
                .HasForeignKey(es => es.ServicioID);

            base.OnModelCreating(modelBuilder);
        }
    }
} 