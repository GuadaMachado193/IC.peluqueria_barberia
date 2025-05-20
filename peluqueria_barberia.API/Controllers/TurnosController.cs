using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using peluqueria_barberia.API.Models;

namespace peluqueria_barberia.API.Controllers
{
    [RoutePrefix("api/turnos")]
    public class TurnosController : BaseController
    {
        // GET: api/turnos
        public HttpResponseMessage Get()
        {
            try
            {
                var turnos = _context.Turnos
                    .Include(t => t.Cliente)
                    .Include(t => t.Empleado)
                    .Include(t => t.Servicio)
                    .OrderByDescending(t => t.Fecha)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, turnos);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/turnos/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var turno = _context.Turnos
                    .Include(t => t.Cliente)
                    .Include(t => t.Empleado)
                    .Include(t => t.Servicio)
                    .FirstOrDefault(t => t.TurnoID == id);

                if (turno == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Turno no encontrado");
                }

                return CreateResponse(HttpStatusCode.OK, turno);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/turnos/fecha/2024-01-01
        [HttpGet]
        [Route("fecha/{fecha}")]
        public HttpResponseMessage GetByFecha(DateTime fecha)
        {
            try
            {
                var turnos = _context.Turnos
                    .Include(t => t.Cliente)
                    .Include(t => t.Empleado)
                    .Include(t => t.Servicio)
                    .Where(t => t.Fecha.Date == fecha.Date)
                    .OrderBy(t => t.HoraInicio)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, turnos);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/turnos/empleado/5/fecha/2024-01-01
        [HttpGet]
        [Route("empleado/{empleadoId}/fecha/{fecha}")]
        public HttpResponseMessage GetByEmpleadoAndFecha(int empleadoId, DateTime fecha)
        {
            try
            {
                var turnos = _context.Turnos
                    .Include(t => t.Cliente)
                    .Include(t => t.Servicio)
                    .Where(t => t.EmpleadoID == empleadoId && t.Fecha.Date == fecha.Date)
                    .OrderBy(t => t.HoraInicio)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, turnos);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/turnos
        public HttpResponseMessage Post([FromBody] Turno turno)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                // Verificar si el empleado está disponible en ese horario
                var turnoExistente = _context.Turnos
                    .Any(t => t.EmpleadoID == turno.EmpleadoID &&
                             t.Fecha.Date == turno.Fecha.Date &&
                             ((turno.HoraInicio >= t.HoraInicio && turno.HoraInicio < t.HoraFin) ||
                              (turno.HoraFin > t.HoraInicio && turno.HoraFin <= t.HoraFin)));

                if (turnoExistente)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "El empleado ya tiene un turno asignado en ese horario");
                }

                _context.Turnos.Add(turno);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.Created, turno);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT: api/turnos/5
        public HttpResponseMessage Put(int id, [FromBody] Turno turno)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                var turnoExistente = _context.Turnos.Find(id);
                if (turnoExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Turno no encontrado");
                }

                // Verificar si el empleado está disponible en el nuevo horario
                var turnoConflictivo = _context.Turnos
                    .Any(t => t.TurnoID != id &&
                             t.EmpleadoID == turno.EmpleadoID &&
                             t.Fecha.Date == turno.Fecha.Date &&
                             ((turno.HoraInicio >= t.HoraInicio && turno.HoraInicio < t.HoraFin) ||
                              (turno.HoraFin > t.HoraInicio && turno.HoraFin <= t.HoraFin)));

                if (turnoConflictivo)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "El empleado ya tiene un turno asignado en ese horario");
                }

                turnoExistente.ClienteID = turno.ClienteID;
                turnoExistente.EmpleadoID = turno.EmpleadoID;
                turnoExistente.ServicioID = turno.ServicioID;
                turnoExistente.Fecha = turno.Fecha;
                turnoExistente.HoraInicio = turno.HoraInicio;
                turnoExistente.HoraFin = turno.HoraFin;
                turnoExistente.Estado = turno.Estado;
                turnoExistente.Notas = turno.Notas;
                turnoExistente.FechaModificacion = DateTime.Now;

                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, turnoExistente);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE: api/turnos/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var turno = _context.Turnos.Find(id);
                if (turno == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Turno no encontrado");
                }

                _context.Turnos.Remove(turno);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, "Turno eliminado correctamente");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
} 