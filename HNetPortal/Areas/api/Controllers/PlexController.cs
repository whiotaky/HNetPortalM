using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WSHLib;

namespace HNetPortal.Areas.api.Controllers {

	[HNetAuthorize(Roles = "Administrators")]
	public class PlexController : ApiController {


		//POST form data
		public HttpResponseMessage Post([FromBody]HNetPortal.Plex.Params @params) {

			Logger.Log($"Post: {@params.plexPathID}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				string  ret = Plex.CopyToHome(@params);
				
				Logger.Log("Returning source file contents");
				httpResponseMessage.Content = new ObjectContent<string>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Exception in api TFS controller", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}

		//Plex refresh
		// GET: api/Plex/5	
		public HttpResponseMessage Get(string id) {

			Logger.Log($"Get= {id}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {

				string ret = Plex.SectionRefresh(id);

				Logger.Log($"Send refresh signal");
				httpResponseMessage.Content = new ObjectContent<string>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}

	}
}
