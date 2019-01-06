using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WSHLib;

namespace HNetPortal.Areas.api.Controllers {
	public class VersionController : ApiController {

		// GET: api/Version
		public HttpResponseMessage Get() {

			AppInfo soapApiInfo = PFTrack2.Info.AppInfo;
			soapApiInfo.Version = PFTrack2.Soap.Version;

			Models.VersionsVM versionsVM = new Models.VersionsVM {
				Portal = Global.PortalInfo,
				WSHLib = Info.AppInfo,
				PFTrack2 = PFTrack2.Info.AppInfo,
				PFTrack2SoapApi = soapApiInfo
			};

			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
			httpResponseMessage.Content = new ObjectContent<Models.VersionsVM>(versionsVM, Configuration.Formatters.JsonFormatter);
			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

			return httpResponseMessage;
		}
	}

}
