using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace peluqueria_barberia.API.Controllers
{
    public class BarberiaPeluqueriaController : ApiController
    {
        // GET: api/BarberiaPeluqueria
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
