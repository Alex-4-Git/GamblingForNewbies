using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GamblingNewbies
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Redirect's Home's About route
            routes.MapRoute(
                name: "AboutRoute",
                url: "About",
                defaults: new { controller = "Home", action = "About" }
            );
            // Redirect's non-param Setion to Forum
            routes.MapRoute(
                name: "NoSectionRoute",
                url: "Forum/Section",
                defaults: new { controller = "Forum", action = "Index" }
            );
            // Redirect's non-param Game to Forum
            routes.MapRoute(
                name: "NoGameRoute",
                url: "Game",
                defaults: new { controller = "Game", action = "Index" }
            );

            // Redirect's non-param Setion to Forum
            routes.MapRoute(
                name: "NoTableRoute",
                url: "Table/Index",
                defaults: new { controller = "Game", action = "Index" }
            );
            routes.MapRoute(
                name: "NoTableRoute2",
                url: "Table",
                defaults: new { controller = "Game", action = "Index" }
            );
            // Redirect's non-param Setion to Forum
            routes.MapRoute(
                name: "TableIDRoute",
                url: "Table/{id}",
                defaults: new { controller = "Table", action = "Index", id = UrlParameter.Optional }
            );

            // Default Route that came with MVC
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            // Show a 404 Error page for anything else?
            //routes.MapRoute(
            //    name: "Error",
            //    url: "{*url}",
            //    defaults: new { controller = "Error", action = "Index", id = "404" }
            //);



            //For Thread action/controller?
            //routes.MapRoute(
            //    name: "Thread",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Thread", action = "Index", id = UrlParameter.Optional }
            //);

        }
    }
}
