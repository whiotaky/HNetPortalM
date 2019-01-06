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
	public class LogTailController : ApiController {

		// POST: api/LogTail
		public HttpResponseMessage Post([FromBody]Models.GenericRequest req) {
			Logger.Log($"POST api/LogTail whichLog={req.numLines}  numLines={req.numLines}");

			MemoryStream ms = Logger.Get(req.whichLog, req.numLines);
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
			httpResponseMessage.Content = new StreamContent(ms);
			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			return httpResponseMessage;
		}

	}
}
