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

        // POST: api/usuarios
        public HttpResponseMessage Post([FromBody] Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de usuario inválidos");
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
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Datos de usuario inválidos");
                }

                if (id != usuario.UsuarioID)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "ID de usuario no coincide");
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

                _context.Entry(usuarioExistente).CurrentValues.SetValues(usuario);
                _context.SaveChanges();

                return CreateResponse(HttpStatusCode.NoContent);
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

                // Verificar si el usuario tiene registros en el historial
                var tieneHistorial = _context.HistorialTurnos.Any(h => h.UsuarioID == id);
                if (tieneHistorial)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "No se puede eliminar el usuario porque tiene registros en el historial");
                }

                _context.Usuarios.Remove(usuario);
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