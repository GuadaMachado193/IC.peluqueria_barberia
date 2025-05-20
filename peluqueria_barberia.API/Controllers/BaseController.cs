using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using peluqueria_barberia.API.Data;

namespace peluqueria_barberia.API.Controllers
{
    public class BaseController : ApiController
    {
        protected readonly BarberiaEsteticaContext _context;

        public BaseController()
        {
            _context = new BarberiaEsteticaContext();
        }

        protected HttpResponseMessage CreateResponse<T>(HttpStatusCode statusCode, T data)
        {
            return Request.CreateResponse(statusCode, data);
        }

        protected HttpResponseMessage CreateErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Request.CreateErrorResponse(statusCode, message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 