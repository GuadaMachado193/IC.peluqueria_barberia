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
    [RoutePrefix("api/roles")]
    public class RolesController : BaseController
    {
        // GET: api/roles
        public HttpResponseMessage Get()
        {
            try
            {
                var roles = _context.Roles
                    .Include(r => r.Usuarios)
                    .ToList();

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
                var rol = _context.Roles
                    .Include(r => r.Usuarios)
                    .FirstOrDefault(r => r.RolID == id);

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
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de rol inválidos");
                }

                // Verificar si el nombre del rol ya existe
                var rolExistente = _context.Roles
                    .Any(r => r.Nombre == rol.Nombre);

                if (rolExistente)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "El nombre del rol ya existe");
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
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de rol inválidos");
                }

                if (id != rol.RolID)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "ID de rol no coincide");
                }

                var rolExistente = _context.Roles.Find(id);
                if (rolExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Rol no encontrado");
                }

                // Verificar si el nuevo nombre del rol ya existe (si se está cambiando)
                if (rolExistente.Nombre != rol.Nombre)
                {
                    var nombreRolExistente = _context.Roles
                        .Any(r => r.Nombre == rol.Nombre);

                    if (nombreRolExistente)
                    {
                        return CreateErrorResponse(HttpStatusCode.BadRequest, "El nombre del rol ya existe");
                    }
                }

                _context.Entry(rolExistente).CurrentValues.SetValues(rol);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.NoContent);
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

                // Verificar si el rol tiene usuarios asociados
                var tieneUsuarios = _context.Usuarios.Any(u => u.RolID == id);
                if (tieneUsuarios)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "No se puede eliminar el rol porque tiene usuarios asociados");
                }

                _context.Roles.Remove(rol);
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