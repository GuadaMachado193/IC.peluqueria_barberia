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
    [RoutePrefix("api/servicios")]
    public class ServiciosController : BaseController
    {
        // GET: api/servicios
        public HttpResponseMessage Get()
        {
            try
            {
                var servicios = _context.Servicios.ToList();
                return CreateResponse(HttpStatusCode.OK, servicios);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/servicios/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var servicio = _context.Servicios
                    .Include(s => s.EmpleadoServicios)
                    .Include(s => s.EmpleadoServicios.Select(es => es.Empleado))
                    .Include(s => s.Turnos)
                    .FirstOrDefault(s => s.ServicioID == id);

                if (servicio == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Servicio no encontrado");
                }

                return CreateResponse(HttpStatusCode.OK, servicio);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/servicios
        public HttpResponseMessage Post([FromBody] Servicio servicio)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de servicio inválidos");
                }

                _context.Servicios.Add(servicio);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.Created, servicio);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT: api/servicios/5
        public HttpResponseMessage Put(int id, [FromBody] Servicio servicio)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de servicio inválidos");
                }

                if (id != servicio.ServicioID)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "ID de servicio no coincide");
                }

                var servicioExistente = _context.Servicios.Find(id);
                if (servicioExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Servicio no encontrado");
                }

                _context.Entry(servicioExistente).CurrentValues.SetValues(servicio);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE: api/servicios/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var servicio = _context.Servicios.Find(id);
                if (servicio == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Servicio no encontrado");
                }

                // Verificar si el servicio tiene turnos asociados
                var tieneTurnos = _context.Turnos.Any(t => t.ServicioID == id);
                if (tieneTurnos)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "No se puede eliminar el servicio porque tiene turnos asociados");
                }

                _context.Servicios.Remove(servicio);
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