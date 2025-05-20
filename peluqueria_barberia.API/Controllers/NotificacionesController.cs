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
    [RoutePrefix("api/notificaciones")]
    public class NotificacionesController : BaseController
    {
        // GET: api/notificaciones
        public HttpResponseMessage Get()
        {
            try
            {
                var notificaciones = _context.Notificaciones
                    .Include(n => n.Turno)
                    .OrderByDescending(n => n.FechaEnvio)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, notificaciones);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/notificaciones/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var notificacion = _context.Notificaciones
                    .Include(n => n.Turno)
                    .FirstOrDefault(n => n.NotificacionID == id);

                if (notificacion == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Notificación no encontrada");
                }

                return CreateResponse(HttpStatusCode.OK, notificacion);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/notificaciones/turno/5
        [HttpGet]
        [Route("turno/{turnoId}")]
        public HttpResponseMessage GetByTurno(int turnoId)
        {
            try
            {
                var notificaciones = _context.Notificaciones
                    .Include(n => n.Turno)
                    .Where(n => n.TurnoID == turnoId)
                    .OrderByDescending(n => n.FechaEnvio)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, notificaciones);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/notificaciones/estado/pendiente
        [HttpGet]
        [Route("estado/{estado}")]
        public HttpResponseMessage GetByEstado(string estado)
        {
            try
            {
                var notificaciones = _context.Notificaciones
                    .Include(n => n.Turno)
                    .Where(n => n.Estado == estado)
                    .OrderByDescending(n => n.FechaEnvio)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, notificaciones);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/notificaciones
        public HttpResponseMessage Post([FromBody] Notificacion notificacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de notificación inválidos");
                }

                // Verificar si el turno existe
                var turno = _context.Turnos.Find(notificacion.TurnoID);
                if (turno == null)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "El turno especificado no existe");
                }

                notificacion.FechaEnvio = DateTime.Now;
                _context.Notificaciones.Add(notificacion);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.Created, notificacion);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT: api/notificaciones/5
        public HttpResponseMessage Put(int id, [FromBody] Notificacion notificacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de notificación inválidos");
                }

                if (id != notificacion.NotificacionID)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "ID de notificación no coincide");
                }

                var notificacionExistente = _context.Notificaciones.Find(id);
                if (notificacionExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Notificación no encontrada");
                }

                _context.Entry(notificacionExistente).CurrentValues.SetValues(notificacion);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE: api/notificaciones/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var notificacion = _context.Notificaciones.Find(id);
                if (notificacion == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Notificación no encontrada");
                }

                _context.Notificaciones.Remove(notificacion);
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