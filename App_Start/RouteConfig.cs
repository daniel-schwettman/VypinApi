using AttributeRouting.Web.Mvc;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace VypinApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapMvcAttributeRoutes();

			routes.MapRoute("TagRequest", "{action}", new
			{
				controller = "Tag",
				action = "Tag",
				id = UrlParameter.Optional
			});

			//routes.MapRoute(
			//	name: "TagRequest",
			//	url: "{action}",
			//	defaults: new { controller = "Tag", action = "Tag", id = UrlParameter.Optional },
			//	namespaces: new[] { "VypinApi.Controllers" }
			//);

			routes.MapRoute(
				name: "Authenticate",
				url: "{action}",
				defaults: new { controller = "User", action = "Authenticate", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
    }
}

