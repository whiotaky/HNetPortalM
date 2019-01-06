using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace HNetPortal {
	
	//This is 1 of 2 classes in place to allow Session data to be saved.
	public class SessionControllerHandler : HttpControllerHandler, IRequiresSessionState {
		public SessionControllerHandler(RouteData routeData)
			: base(routeData) { }
	}

	//This is 2 of 2 classes in place to allow Session data to be saved.
	public class SessionHttpControllerRouteHandler : HttpControllerRouteHandler {
		protected override IHttpHandler GetHttpHandler(RequestContext requestContext) {
			return new SessionControllerHandler(requestContext.RouteData);
		}
	}

	public static class WebApiConfig {

		public static void Register(HttpConfiguration config) {

			//This block sets up Session to work in WebApi
			var httpControllerRouteHandler = 
				typeof(HttpControllerRouteHandler).GetField("_instance",
				System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

			if (httpControllerRouteHandler != null) {
				httpControllerRouteHandler.SetValue(null,
					new Lazy<HttpControllerRouteHandler>(() => new SessionHttpControllerRouteHandler(), true));
			}

			// Web API configuration and services

			//Need Cross-Origin Resource Sharing?  Then add library and enable it here
			//https://www.infoworld.com/article/3173363/application-development/how-to-enable-cors-on-your-web-api.html
			//config.EnableCors();
			// Web API routes
			config.MapHttpAttributeRoutes();
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);


			//Cause output to be Json by default.
			//https://stackoverflow.com/questions/9847564/how-do-i-get-asp-net-web-api-to-return-json-instead-of-xml-using-chrome
			GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(
				new System.Net.Http.Formatting.RequestHeaderMapping(
							"Accept",
							  "text/html", StringComparison.InvariantCultureIgnoreCase,
							  true, "application/json"
				)
			);

		}
	}
}