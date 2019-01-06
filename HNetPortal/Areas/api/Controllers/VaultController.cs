using KeePassLib;
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

	[HNetAuthorize]
	public class VaultController : ApiController {

		// GET: api/Vault/5	
		public HttpResponseMessage Get(string id) {

			Logger.Log($"Get= {id}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				string dbpath = (string)ConfigurationManager.AppSettings["KEEPASSPATH"];
				KeePass kp = new KeePass(dbpath, Global.PortalSettings.KeePassDBPassword);
				KeePass.VaultEntryItem vei = kp.EncryptEntry(kp.FindByUUID(id));

				Logger.Log($"Got the requested vault entry item");
				httpResponseMessage.Content = new ObjectContent<KeePass.VaultEntryItem>(vei, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}


	}
}
