using System;
using System.Collections.Generic;
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

        // POST: api/turnos
        public HttpResponseMessage Post([FromBody] Turno turno)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de turno inválidos");
                }

                // Validar disponibilidad del turno
                var turnoExistente = _context.Turnos
                    .Any(t => t.EmpleadoID == turno.EmpleadoID &&
                             t.Fecha == turno.Fecha &&
                             ((t.HoraInicio <= turno.HoraInicio && t.HoraFin > turno.HoraInicio) ||
                              (t.HoraInicio < turno.HoraFin && t.HoraFin >= turno.HoraFin)));

                if (turnoExistente)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Ya existe un turno en ese horario para el empleado");
                }

                turno.FechaModificacion = DateTime.Now;
                _context.Turnos.Add(turno);
                _context.SaveChanges();

                // Crear registro en historial
                var historial = new HistorialTurno
                {
                    TurnoID = turno.TurnoID,
                    UsuarioID = 1, // TODO: Obtener del usuario autenticado
                    Accion = "Creado",
                    FechaCambio = DateTime.Now
                };
                _context.HistorialTurnos.Add(historial);
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
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de turno inválidos");
                }

                if (id != turno.TurnoID)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "ID de turno no coincide");
                }

                var turnoExistente = _context.Turnos.Find(id);
                if (turnoExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Turno no encontrado");
                }

                // Validar disponibilidad del turno
                var turnoSuperpuesto = _context.Turnos
                    .Any(t => t.TurnoID != id &&
                             t.EmpleadoID == turno.EmpleadoID &&
                             t.Fecha == turno.Fecha &&
                             ((t.HoraInicio <= turno.HoraInicio && t.HoraFin > turno.HoraInicio) ||
                              (t.HoraInicio < turno.HoraFin && t.HoraFin >= turno.HoraFin)));

                if (turnoSuperpuesto)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Ya existe un turno en ese horario para el empleado");
                }

                turno.FechaModificacion = DateTime.Now;
                _context.Entry(turnoExistente).CurrentValues.SetValues(turno);
                _context.SaveChanges();

                // Crear registro en historial
                var historial = new HistorialTurno
                {
                    TurnoID = turno.TurnoID,
                    UsuarioID = 1, // TODO: Obtener del usuario autenticado
                    Accion = "Modificado",
                    FechaCambio = DateTime.Now
                };
                _context.HistorialTurnos.Add(historial);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.NoContent);
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

                // Crear registro en historial
                var historial = new HistorialTurno
                {
                    TurnoID = turno.TurnoID,
                    UsuarioID = 1, // TODO: Obtener del usuario autenticado
                    Accion = "Eliminado",
                    FechaCambio = DateTime.Now
                };
                _context.HistorialTurnos.Add(historial);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
} 