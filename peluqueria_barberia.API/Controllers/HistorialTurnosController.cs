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
    [RoutePrefix("api/historial-turnos")]
    public class HistorialTurnosController : BaseController
    {
        // GET: api/historial-turnos
        public HttpResponseMessage Get()
        {
            try
            {
                var historial = _context.HistorialTurnos
                    .Include(h => h.Turno)
                    .Include(h => h.Usuario)
                    .OrderByDescending(h => h.FechaCambio)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, historial);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/historial-turnos/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var historial = _context.HistorialTurnos
                    .Include(h => h.Turno)
                    .Include(h => h.Usuario)
                    .FirstOrDefault(h => h.HistorialID == id);

                if (historial == null)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, "Registro de historial no encontrado");
                }

                return CreateResponse(HttpStatusCode.OK, historial);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/historial-turnos/turno/5
        [HttpGet]
        [Route("turno/{turnoId}")]
        public HttpResponseMessage GetByTurno(int turnoId)
        {
            try
            {
                var historial = _context.HistorialTurnos
                    .Include(h => h.Turno)
                    .Include(h => h.Usuario)
                    .Where(h => h.TurnoID == turnoId)
                    .OrderByDescending(h => h.FechaCambio)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, historial);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/historial-turnos/usuario/5
        [HttpGet]
        [Route("usuario/{usuarioId}")]
        public HttpResponseMessage GetByUsuario(int usuarioId)
        {
            try
            {
                var historial = _context.HistorialTurnos
                    .Include(h => h.Turno)
                    .Include(h => h.Usuario)
                    .Where(h => h.UsuarioID == usuarioId)
                    .OrderByDescending(h => h.FechaCambio)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, historial);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: api/historial-turnos/fecha/2024-01-01
        [HttpGet]
        [Route("fecha/{fecha}")]
        public HttpResponseMessage GetByFecha(DateTime fecha)
        {
            try
            {
                var historial = _context.HistorialTurnos
                    .Include(h => h.Turno)
                    .Include(h => h.Usuario)
                    .Where(h => h.FechaCambio.Date == fecha.Date)
                    .OrderByDescending(h => h.FechaCambio)
                    .ToList();

                return CreateResponse(HttpStatusCode.OK, historial);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
} 