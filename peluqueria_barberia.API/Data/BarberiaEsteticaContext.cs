using System.Data.Entity;
using peluqueria_barberia.API.Models;

namespace peluqueria_barberia.API.Data
{
    public class BarberiaEsteticaContext : DbContext
    {
        public BarberiaEsteticaContext() : base("name=BarberiaEsteticaContext")
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<EstadoTurno> EstadosTurno { get; set; }
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
            // Configuración de relaciones y restricciones
            modelBuilder.Entity<Turno>()
                .HasRequired(t => t.Cliente)
                .WithMany(c => c.Turnos)
                .HasForeignKey(t => t.ClienteID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Turno>()
                .HasRequired(t => t.Empleado)
                .WithMany(e => e.Turnos)
                .HasForeignKey(t => t.EmpleadoID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Turno>()
                .HasRequired(t => t.Servicio)
                .WithMany(s => s.Turnos)
                .HasForeignKey(t => t.ServicioID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HistorialTurno>()
                .HasRequired(h => h.Turno)
                .WithMany(t => t.HistorialTurnos)
                .HasForeignKey(h => h.TurnoID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HistorialTurno>()
                .HasRequired(h => h.Usuario)
                .WithMany(u => u.HistorialTurnos)
                .HasForeignKey(h => h.UsuarioID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Notificacion>()
                .HasRequired(n => n.Turno)
                .WithMany(t => t.Notificaciones)
                .HasForeignKey(n => n.TurnoID)
                .WillCascadeOnDelete(false);

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