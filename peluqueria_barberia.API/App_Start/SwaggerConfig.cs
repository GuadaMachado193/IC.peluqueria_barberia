using System.Web.Http;
using WebActivatorEx;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(peluqueria_barberia.API.App_Start.SwaggerConfig), "Register")]

namespace peluqueria_barberia.API.App_Start
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Peluqueria Barberia API");
                    c.IncludeXmlComments(string.Format(@"{0}\bin\peluqueria_barberia.API.XML",
                        System.AppDomain.CurrentDomain.BaseDirectory));
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("Peluqueria Barberia API Documentation");
                });
        }
    }
}
