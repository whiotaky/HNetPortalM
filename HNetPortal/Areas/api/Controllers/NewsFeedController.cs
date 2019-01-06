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
	public class NewsFeedController : ApiController {

		public HttpResponseMessage Get(int id) {

			Logger.Log($"GET api/NewsFeed/" + id);

			MemoryStream ms = NewsFeed.Get(id);
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
			httpResponseMessage.Content = new StreamContent(ms);
			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			return httpResponseMessage;
		}
	}
}
