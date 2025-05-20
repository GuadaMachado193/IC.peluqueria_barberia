using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using peluqueria_barberia.API.Models;

namespace peluqueria_barberia.API.Controllers
{
    [RoutePrefix("api/roles")]
    public class RolesController : BaseController
    {
        // GET: api/roles
        public HttpResponseMessage Get()
        {
            try
            {
                var roles = _context.Roles.ToList();
                return CreateResponse(HttpStatusCode.OK, roles);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/roles/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var rol = _context.Roles.Find(id);
                if (rol == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Rol no encontrado");
                }
                return CreateResponse(HttpStatusCode.OK, rol);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/roles
        public HttpResponseMessage Post([FromBody] Rol rol)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                _context.Roles.Add(rol);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.Created, rol);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT: api/roles/5
        public HttpResponseMessage Put(int id, [FromBody] Rol rol)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                var rolExistente = _context.Roles.Find(id);
                if (rolExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Rol no encontrado");
                }

                rolExistente.Nombre = rol.Nombre;
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, rolExistente);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE: api/roles/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var rol = _context.Roles.Find(id);
                if (rol == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Rol no encontrado");
                }

                _context.Roles.Remove(rol);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, "Rol eliminado correctamente");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
} 