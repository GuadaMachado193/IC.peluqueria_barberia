using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using peluqueria_barberia.API.Models;

namespace peluqueria_barberia.API.Controllers
{
    [RoutePrefix("api/usuarios")]
    public class UsuariosController : BaseController
    {
        // GET: api/usuarios
        public HttpResponseMessage Get()
        {
            try
            {
                var usuarios = _context.Usuarios
                    .Include(u => u.Rol)
                    .ToList();
                return CreateResponse(HttpStatusCode.OK, usuarios);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/usuarios/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var usuario = _context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefault(u => u.UsuarioID == id);

                if (usuario == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Usuario no encontrado");
                }

                return CreateResponse(HttpStatusCode.OK, usuario);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/usuarios/por-rol/5
        [HttpGet]
        [Route("por-rol/{rolId}")]
        public HttpResponseMessage GetByRol(int rolId)
        {
            try
            {
                var usuarios = _context.Usuarios
                    .Include(u => u.Rol)
                    .Where(u => u.RolID == rolId)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, usuarios);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/usuarios
        public HttpResponseMessage Post([FromBody] Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                // Verificar si el nombre de usuario ya existe
                var usuarioExistente = _context.Usuarios
                    .Any(u => u.Usuario == usuario.Usuario);

                if (usuarioExistente)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "El nombre de usuario ya existe");
                }

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.Created, usuario);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT: api/usuarios/5
        public HttpResponseMessage Put(int id, [FromBody] Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos inválidos");
                }

                var usuarioExistente = _context.Usuarios.Find(id);
                if (usuarioExistente == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Usuario no encontrado");
                }

                // Verificar si el nuevo nombre de usuario ya existe (si se está cambiando)
                if (usuarioExistente.Usuario != usuario.Usuario)
                {
                    var nombreUsuarioExistente = _context.Usuarios
                        .Any(u => u.Usuario == usuario.Usuario);

                    if (nombreUsuarioExistente)
                    {
                        return CreateErrorResponse(HttpStatusCode.BadRequest, "El nombre de usuario ya existe");
                    }
                }

                usuarioExistente.Usuario = usuario.Usuario;
                usuarioExistente.Email = usuario.Email;
                usuarioExistente.Apellido = usuario.Apellido;
                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Telefono = usuario.Telefono;
                usuarioExistente.Estado = usuario.Estado;
                usuarioExistente.RolID = usuario.RolID;

                if (!string.IsNullOrEmpty(usuario.Clave))
                {
                    usuarioExistente.Clave = usuario.Clave;
                }

                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, usuarioExistente);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE: api/usuarios/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var usuario = _context.Usuarios.Find(id);
                if (usuario == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Usuario no encontrado");
                }

                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.OK, "Usuario eliminado correctamente");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
} 