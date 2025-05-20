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
                var cliente = _context.Clientes
                    .Include(c => c.Turnos)
                    .FirstOrDefault(c => c.ClienteID == id);

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

        // POST: api/clientes
        public HttpResponseMessage Post([FromBody] Cliente cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de cliente inválidos");
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
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de cliente inválidos");
                }

                if (id != cliente.ClienteID)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "ID de cliente no coincide");
                }

                var clienteExistente = _context.Clientes.Find(id);
                if (clienteExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Cliente no encontrado");
                }

                _context.Entry(clienteExistente).CurrentValues.SetValues(cliente);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.NoContent);
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

                // Verificar si el cliente tiene turnos asociados
                var tieneTurnos = _context.Turnos.Any(t => t.ClienteID == id);
                if (tieneTurnos)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "No se puede eliminar el cliente porque tiene turnos asociados");
                }

                _context.Clientes.Remove(cliente);
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