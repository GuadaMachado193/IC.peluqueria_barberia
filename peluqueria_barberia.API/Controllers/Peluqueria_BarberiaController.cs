using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace peluqueria_barberia.API.Controllers
{
    public class Peluqueria_BarberiaController : ApiController
    {
        // GET: api/Peluqueria_Barberia
        public List<Clientes> Get()
        {
            List<Clientes> oList = new List<Clientes>();

            using (BarberiaEsteticaEntities db = new BarberiaEsteticaEntities())
            { 
            
            
            
            
            }



            return oList;
        }

        // GET: api/Peluqueria_Barberia/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Peluqueria_Barberia
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Peluqueria_Barberia/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Peluqueria_Barberia/5
        public void Delete(int id)
        {
        }
    }
}
