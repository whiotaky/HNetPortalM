using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WSHLib;



namespace HNetPortal.Areas.api.Controllers {
	public class AuthenticationController : ApiController {

		public HttpResponseMessage Get() {

			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
			try {

				string sessEncKey = Crypto.SessionEncKey();
				Logger.Log($"Returning sessionEncodingKey: {sessEncKey} ");
				httpResponseMessage.Content = new StringContent(sessEncKey, System.Text.Encoding.UTF8, "text/plain");
				
			} catch(Exception ex) {
				Logger.LogException($"exception in controller: {ex.Message}",ex);
				httpResponseMessage.Content = new StringContent("Exception!", System.Text.Encoding.UTF8, "text/plain");
			}

			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			return httpResponseMessage;
		}

	}
}