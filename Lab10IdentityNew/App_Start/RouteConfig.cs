using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lab10IdentityNew
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Custom",
                url: "Article/ArticlesByCategory/{id}/{offset}/{pageSize}",
                defaults: new { controller = "Article", action = "ArticlesByCategory", id = UrlParameter.Optional,
                    offset = UrlParameter.Optional, pageSize = UrlParameter.Optional}
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
