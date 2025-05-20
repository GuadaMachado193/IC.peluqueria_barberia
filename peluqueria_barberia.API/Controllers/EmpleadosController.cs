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
    [RoutePrefix("api/empleados")]
    public class EmpleadosController : BaseController
    {
        // GET: api/empleados
        public HttpResponseMessage Get()
        {
            try
            {
                var empleados = _context.Empleados.ToList();
                return CreateResponse(HttpStatusCode.OK, empleados);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/empleados/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var empleado = _context.Empleados.Find(id);
                if (empleado == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Empleado no encontrado");
                }
                return CreateResponse(HttpStatusCode.OK, empleado);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/empleados/5/turnos
        [HttpGet]
        [Route("{id}/turnos")]
        public HttpResponseMessage GetTurnos(int id)
        {
            try
            {
                var turnos = _context.Turnos
                    .Include(t => t.Cliente)
                    .Include(t => t.Servicio)
                    .Where(t => t.EmpleadoID == id)
                    .OrderByDescending(t => t.Fecha)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, turnos);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/empleados/activos
        [HttpGet]
        [Route("activos")]
        public HttpResponseMessage GetActivos()
        {
            try
            {
                var empleados = _context.Empleados
                    .Where(e => e.Estado == "activo")
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, empleados);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/empleados
        public HttpResponseMessage Post([FromBody] Empleado empleado)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                _context.Empleados.Add(empleado);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.Created, empleado);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT: api/empleados/5
        public HttpResponseMessage Put(int id, [FromBody] Empleado empleado)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                var empleadoExistente = _context.Empleados.Find(id);
                if (empleadoExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Empleado no encontrado");
                }

                empleadoExistente.Apellido = empleado.Apellido;
                empleadoExistente.Nombre = empleado.Nombre;
                empleadoExistente.Telefono = empleado.Telefono;
                empleadoExistente.HorarioInicio = empleado.HorarioInicio;
                empleadoExistente.HorarioFin = empleado.HorarioFin;
                empleadoExistente.Estado = empleado.Estado;

                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, empleadoExistente);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE: api/empleados/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var empleado = _context.Empleados.Find(id);
                if (empleado == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Empleado no encontrado");
                }

                // Verificar si el empleado tiene turnos
                var tieneTurnos = _context.Turnos.Any(t => t.EmpleadoID == id);
                if (tieneTurnos)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "No se puede eliminar el empleado porque tiene turnos asociados");
                }

                _context.Empleados.Remove(empleado);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, "Empleado eliminado correctamente");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/empleados/5/servicios
        [HttpPost]
        [Route("{id}/servicios")]
        public HttpResponseMessage AgregarServicio(int id, [FromBody] int servicioId)
        {
            try
            {
                var empleado = _context.Empleados.Find(id);
                if (empleado == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Empleado no encontrado");
                }

                var servicio = _context.Servicios.Find(servicioId);
                if (servicio == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Servicio no encontrado");
                }

                var empleadoServicio = new EmpleadoServicio
                {
                    EmpleadoID = id,
                    ServicioID = servicioId
                };

                _context.EmpleadoServicios.Add(empleadoServicio);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE: api/empleados/5/servicios/3
        [HttpDelete]
        [Route("{id}/servicios/{servicioId}")]
        public HttpResponseMessage EliminarServicio(int id, int servicioId)
        {
            try
            {
                var empleadoServicio = _context.EmpleadoServicios
                    .FirstOrDefault(es => es.EmpleadoID == id && es.ServicioID == servicioId);

                if (empleadoServicio == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Relación empleado-servicio no encontrada");
                }

                _context.EmpleadoServicios.Remove(empleadoServicio);
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