using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity; // Importa el espacio de nombres EntityFramework
namespace peluqueria_barberia.API.Controllers
{
    public class BarberiaPeluqueriaController : ApiController
    {
        // GET: api/BarberiaPeluqueria
        // GET: api/BarberiaPeluqueria
        public List<Turnos> Get()
        {
            List<Turnos> oList = new List<Turnos>();
            using (BarberiaEsteticaEntities1 db = new BarberiaEsteticaEntities1())
            {
                oList = db.Turnos
                    .Include(t => t.Clientes)    // Carga eager del Cliente
                    .Include(t => t.Empleados)   // Carga eager del Empleado
                    .Include(t => t.Servicios)   // Carga eager del Servicio
                    .ToList();
            }
            return oList;
        }

        // GET: api/BarberiaPeluqueria/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/BarberiaPeluqueria
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/BarberiaPeluqueria/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/BarberiaPeluqueria/5
        public void Delete(int id)
        {
        }
    }
}
