using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WSHLib;

namespace HNetPortal.Areas.api.Controllers {
	
	[HNetAuthorize]
	[RoutePrefix("api/ICal")]
	public class ICalController : ApiController {

		[Route("ParseFile")]
		[HttpPost]
		public HttpResponseMessage ParseFile([FromBody]Models.GenericRequest req) {

			string fileName = req.sourceFileName;

			Logger.Log($"Parse: {fileName}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
		
			try {
				List<ICal.ICalItem> ret = ICal.ParseFile(fileName);
				Logger.Log($"Got the list of links");
				httpResponseMessage.Content = new ObjectContent<List<ICal.ICalItem>>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
				httpResponseMessage.Content = new ObjectContent<string>(ex.Message, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");	
			}

			return httpResponseMessage;

		}

	}
}
