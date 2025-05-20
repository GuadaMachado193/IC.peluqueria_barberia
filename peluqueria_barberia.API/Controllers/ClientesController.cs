using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using peluqueria_barberia.API.Models;

namespace peluqueria_barberia.API.Controllers
{
    [RoutePrefix("api/clientes")]
    public class ClientesController : BaseController
    {
        // GET: api/clientes
        public HttpResponseMessage Get()
        {
            try
            {
                var clientes = _context.Clientes.ToList();
                return CreateResponse(HttpStatusCode.OK, clientes);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/clientes/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var cliente = _context.Clientes.Find(id);
                if (cliente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Cliente no encontrado");
                }
                return CreateResponse(HttpStatusCode.OK, cliente);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/clientes/5/turnos
        [HttpGet]
        [Route("{id}/turnos")]
        public HttpResponseMessage GetTurnos(int id)
        {
            try
            {
                var turnos = _context.Turnos
                    .Include(t => t.Empleado)
                    .Include(t => t.Servicio)
                    .Where(t => t.ClienteID == id)
                    .OrderByDescending(t => t.Fecha)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, turnos);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/clientes
        public HttpResponseMessage Post([FromBody] Cliente cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                _context.Clientes.Add(cliente);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.Created, cliente);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT: api/clientes/5
        public HttpResponseMessage Put(int id, [FromBody] Cliente cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                var clienteExistente = _context.Clientes.Find(id);
                if (clienteExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Cliente no encontrado");
                }

                clienteExistente.Apellido = cliente.Apellido;
                clienteExistente.Nombre = cliente.Nombre;
                clienteExistente.Telefono = cliente.Telefono;
                clienteExistente.Email = cliente.Email;

                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, clienteExistente);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE: api/clientes/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var cliente = _context.Clientes.Find(id);
                if (cliente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Cliente no encontrado");
                }

                // Verificar si el cliente tiene turnos
                var tieneTurnos = _context.Turnos.Any(t => t.ClienteID == id);
                if (tieneTurnos)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "No se puede eliminar el cliente porque tiene turnos asociados");
                }

                _context.Clientes.Remove(cliente);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, "Cliente eliminado correctamente");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
} 