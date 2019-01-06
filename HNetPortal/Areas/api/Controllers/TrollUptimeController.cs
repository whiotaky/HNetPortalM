using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WSHLib;

namespace HNetPortal.Areas.api.Controllers {

	[HNetAuthorize]
	public class TrollUptimeController : ApiController {

		public HttpResponseMessage Get() {
			Logger.Log($"GET api/TrollUptime");

			string privateKeyFileName = System.Web.HttpContext.Current.Server.MapPath(@"~/App_Data") + "/boil_id_rsa";
			MemoryStream ms = WSHLib.Network.LinuxUptime.GetTroll(privateKeyFileName);
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
			httpResponseMessage.Content = new StreamContent(ms);
			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			return httpResponseMessage;
		}

	}
}
