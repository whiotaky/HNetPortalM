using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WSHLib;

namespace HNetPortal.Areas.api.Controllers {

	[HNetAuthorize(Roles = "Administrators")]
	public class RSSMasterController : ApiController {

		//Note: there is no method that returns a list since FeedMasterEdit.aspx serves this list
		//by traditional webforms means.
		//
		// GET: api/RSSMaster
		//public IEnumerable<string> Get() {
		//	return new string[] { "value1", "value2" };
		//}


		// GET: api/RSSMaster/5
		public HttpResponseMessage Get(int id) {

			Logger.Log($"RSSMaster Get for {id}");		
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				RSSMasterItem ret = RSSMaster.GetItem(id);
				Logger.Log($"Returning RSSMasterItem name={ret.feedName}");				
				httpResponseMessage.Content = new ObjectContent<RSSMasterItem>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("RSSMasterItem-Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}
			
			return httpResponseMessage;
		}

		// POST: api/RSSMaster
		public HttpResponseMessage Post([FromBody] RSSMasterItem updRec) {

			Logger.Log($"RSSMaster Post for {updRec.feedid}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				RSSMasterItem ret = RSSMaster.UpdateItem(updRec);
				Logger.Log($"RSSMasterItem update success for feed name={updRec.feedName}");				
				httpResponseMessage.Content = new ObjectContent<RSSMasterItem>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("RSSMasterItem-Post Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}

		// PUT: api/RSSMaster/5
		public HttpResponseMessage Put([FromBody]RSSMasterItem putRec) {

			Logger.Log($"RSSMaster Put for {putRec.feedid}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				RSSMasterItem ret = RSSMaster.AddItem(putRec);
				Logger.Log($"RSSMasterItem put success for new feed name={putRec.feedName}");
				httpResponseMessage.Content = new ObjectContent<RSSMasterItem>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("RSSMasterItem-Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}

		// DELETE: api/RSSMaster/5
		public HttpResponseMessage Delete(int id) {

			Logger.Log($"RSSMaster delete for {id}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				bool res = RSSMaster.DeleteItem(id);
				if (res) {
					Logger.Log($"RSSMasterItem delete success for feed id={id}");
					httpResponseMessage.Content = new ObjectContent<string>($"Success deleting {id}", Configuration.Formatters.JsonFormatter);
					httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
				} else {
					httpResponseMessage = Request.CreateResponse(HttpStatusCode.NoContent);
					Logger.Log($"RSSMasterItem delete failed for feed id={id}");
				}
			} catch (Exception ex) {
				Logger.LogException($"RSSMasterItem-Delete Exception feedid={id}", ex);
				Logger.Log($"RSSMasterItem delete exception for feed id={id}");
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}
	}
}
