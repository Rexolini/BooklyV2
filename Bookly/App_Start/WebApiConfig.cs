using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;


namespace Bookly.App_Start
{
    public class WebApiConfig : System.Web.HttpApplication
    {
        public static void Rejestracja(HttpConfiguration config)
        {
            
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = System.Web.Http.RouteParameter.Optional }
            );
        }
    }
}