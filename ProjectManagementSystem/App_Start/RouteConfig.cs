using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectManagementSystem
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{folder}/{*pathInfo}", new { folder = "Content" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "PMS08001", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default2",
                url: "{controller}/{action}/{id}/{key}",
                defaults: new { controller = "PMS08001", action = "Index", id = UrlParameter.Optional, key = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default3",
                url: "{controller}/{action}/{id}/{key}/{key2}",
                defaults: new { controller = "PMS08001", action = "Index", id = UrlParameter.Optional, key = UrlParameter.Optional, key2 = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default4",
                url: "{controller}/{action}/{id}/{key}/{key2}/{key3}",
                defaults: new { controller = "PMS08001", action = "Index", id = UrlParameter.Optional, key = UrlParameter.Optional, key2 = UrlParameter.Optional, key3 = UrlParameter.Optional }
            );

        }
    }
}