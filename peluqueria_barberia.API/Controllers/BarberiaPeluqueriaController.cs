using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity; // Importa el espacio de nombres EntityFramework
namespace peluqueria_barberia.API.Controllers
{
    public class BarberiaPeluqueriaController : ApiController
    {
        // GET: api/BarberiaPeluqueria
        // GET: api/BarberiaPeluqueria
        public List<Turnos> Get()
        {
            List<Turnos> oList = new List<Turnos>();
            using (BarberiaEsteticaEntities1 db = new BarberiaEsteticaEntities1())
            {
                oList = db.Turnos
                    .Include(t => t.Clientes)    // Carga eager del Cliente
                    .Include(t => t.Empleados)   // Carga eager del Empleado
                    .Include(t => t.Servicios)   // Carga eager del Servicio
                    .ToList();
            }
            return oList;
        }

        // GET: api/BarberiaPeluqueria/5
        public Turnos Get(int id)
        {
            using (BarberiaEsteticaEntities1 db = new BarberiaEsteticaEntities1()) // Reemplaza con tu DbContext
            {
                Turnos turno = db.Turnos
                    .Include(t => t.Clientes)
                    .Include(t => t.Empleados)
                    .Include(t => t.Servicios)
                    .FirstOrDefault(t => t.TurnoID == id);

                if (turno == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound); // Devuelve 404 si no se encuentra
                }

                return turno;
            }
        }
      
            // POST: api/BarberiaPeluqueria
            public HttpResponseMessage Post([FromBody] Turnos nuevoTurno)
            {
                using (BarberiaEsteticaEntities1 db = new BarberiaEsteticaEntities1()) // Reemplaza con tu DbContext
                {
                    if (!ModelState.IsValid)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }

                    db.Turnos.Add(nuevoTurno);
                    db.SaveChanges();

                    // Devolver una respuesta 201 (Created) con la ubicación del nuevo recurso
                    var response = Request.CreateResponse(HttpStatusCode.Created);
                    string uri = Url.Link("DefaultApi", new { id = nuevoTurno.TurnoID }); // Genera la URL del nuevo turno
                    response.Headers.Location = new Uri(uri);
                    return response;
                }
            }


        public HttpResponseMessage Put(int id, [FromBody] Turnos turnoActualizado)
        {
            using (BarberiaEsteticaEntities1 db = new BarberiaEsteticaEntities1()) // Crea una instancia del DbContext.
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState); // Si el modelo (turnoActualizado) no es válido, devuelve un error 400.
                }

                if (id != turnoActualizado.TurnoID)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "ID mismatch"); // Si el ID en la URL no coincide con el ID en el cuerpo de la solicitud, devuelve un error 400.
                }

                var turnoExistente = db.Turnos.FirstOrDefault(t => t.TurnoID == id); // Busca el turno existente por su ID.
                if (turnoExistente == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound); // Si no se encuentra el turno, devuelve un error 404 (No Encontrado).
                }

                // Actualiza las propiedades del turno existente con los valores del turno actualizado.
                turnoExistente.ClienteID = turnoActualizado.ClienteID;
                turnoExistente.EmpleadoID = turnoActualizado.EmpleadoID;
                turnoExistente.ServicioID = turnoActualizado.ServicioID;
                turnoExistente.Fecha = turnoActualizado.Fecha;
                turnoExistente.HoraInicio = turnoActualizado.HoraInicio;
                turnoExistente.HoraFin = turnoActualizado.HoraFin;
                turnoExistente.Estado = turnoActualizado.Estado;
                turnoExistente.FechaModificacion = DateTime.Now; // Actualiza la fecha de modificación.

                db.Entry(turnoExistente).State = EntityState.Modified; // Marca la entidad como modificada (aunque no es estrictamente necesario en este caso).
                db.SaveChanges(); // Guarda los cambios en la base de datos.

                return Request.CreateResponse(HttpStatusCode.NoContent); // Devuelve una respuesta 204 (Sin Contenido) para indicar éxito.
            }
        }

        public HttpResponseMessage Delete(int id)
        {
            using (BarberiaEsteticaEntities1 db = new BarberiaEsteticaEntities1()) // Crea una instancia del DbContext.
            {
                var turnoExistente = db.Turnos.FirstOrDefault(t => t.TurnoID == id); // Busca el turno existente por su ID.
                if (turnoExistente == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound); // Si no se encuentra el turno, devuelve un error 404 (No Encontrado).
                }

                db.Turnos.Remove(turnoExistente); // Elimina el turno del contexto de Entity Framework.
                db.SaveChanges();                // Guarda los cambios en la base de datos (ejecuta la instrucción DELETE).

                return Request.CreateResponse(HttpStatusCode.NoContent); // Devuelve una respuesta 204 (Sin Contenido) para indicar éxito.
            }
        }
    }
}
