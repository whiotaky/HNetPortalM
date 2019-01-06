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
	[RoutePrefix("api/UserLinks")]
	public class UserLinksController : ApiController {

		// GET: api/UserLinks/Master/List
		[Route("Master/List")]
		[HttpGet]
		public HttpResponseMessage ListSection() {

			Logger.Log($"ListSection");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				List<UserLinkSecItem> ret =UserLinks.GetSections();
				Logger.Log($"Got the list of Sections");				
				httpResponseMessage.Content = new ObjectContent<List<UserLinkSecItem>>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}
			
			return httpResponseMessage;
		}

		// GET: api/UserLinks/Master/5
		[Route("Master/{id}")]
		[HttpGet]
		public HttpResponseMessage GetSection(int id) {

			Logger.Log($"GetSection sectionid={id}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				UserLinkSecItem ret = UserLinks.GetSection(id);
				Logger.Log($"Got the requested section");
				httpResponseMessage.Content = new ObjectContent<UserLinkSecItem>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}

		// DELETE: api/UserLinls/Master/5
		[Route("Master/{id}")]
		[HttpDelete]
		public HttpResponseMessage DeleteSection(int id) {

			Logger.Log($"DeleteSection for {id}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				bool res = UserLinks.DeleteSection(id);
				if (res) {
					Logger.Log($"Delete success for section id={id}");
					httpResponseMessage.Content = new ObjectContent<string>($"Success deleting {id}", Configuration.Formatters.JsonFormatter);
					httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
				} else {
					httpResponseMessage = Request.CreateResponse(HttpStatusCode.NoContent);
					Logger.Log($"Failed for section id={id}");
				}
			} catch (Exception ex) {
				Logger.LogException($"Delete Exception section id={id}", ex);
				Logger.Log($"Delete exception for section id={id}");
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}


		// POST: api/UserLinks/Master (Update)
		[Route("Master/")]
		[HttpPost]
		public HttpResponseMessage PostSection([FromBody] UserLinkSecItem updRec) {

			Logger.Log($"(Update) for {updRec.sectionid}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				UserLinkSecItem ret = UserLinks.UpdateSection(updRec);

				Logger.Log($"Update success for section id={updRec.sectionid}");
				httpResponseMessage.Content = new ObjectContent<UserLinkSecItem>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Post Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}


		// PUT: api/UserLinks/Master (Add)
		[Route("Master/")]
		[HttpPut]
		public HttpResponseMessage PutSection([FromBody] UserLinkSecItem addRec) {

			Logger.Log($"Add for {addRec.sectionid}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				UserLinkSecItem ret = UserLinks.AddSection(addRec);

				Logger.Log($"Add success for section id={addRec.sectionid}");
				httpResponseMessage.Content = new ObjectContent<UserLinkSecItem>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Put Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}


		// GET: api/UserLinks/User/5
		[Route("User/List/{sectionid}")]
		[HttpGet]
		public HttpResponseMessage ListLinks(int sectionid) {

			Logger.Log($"ListLinks section={sectionid}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				List<UserLinkItem> ret = UserLinks.GetLinks(sectionid);
				Logger.Log($"Got the list of links");
				httpResponseMessage.Content = new ObjectContent<List<UserLinkItem>>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;

		}

		// GET: api/UserLinks/User/Link/5/0
		[Route("User/Link/{sectionid}/{linkid}")]
		[HttpGet]
		public HttpResponseMessage GetLink(int sectionid, int linkid) {

			Logger.Log($"GetLink sectionid={sectionid}, link={linkid} ");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				UserLinkItem ret = UserLinks.GetLink(sectionid, linkid);
				Logger.Log($"Got the requested link");
				httpResponseMessage.Content = new ObjectContent<UserLinkItem>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}

		// POST: api/UserLinks/User/Link/5/0 (Update)
		[Route("User/Link/{sectionid}/{linkid}")]
		[HttpPost]
		public HttpResponseMessage PostLink([FromBody] UserLinkItem updRec) {

			Logger.Log($"(Update) for {updRec.sectionid}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				UserLinkItem ret = UserLinks.UpdateLink(updRec);

				Logger.Log($"Update success for section id={updRec.sectionid}");
				httpResponseMessage.Content = new ObjectContent<UserLinkItem>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Post Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}

		
		// POST: api/UserLinks/User/Link/5/0 (Add)
		[Route("User/Link/{sectionid}/{linkid}")]
		[HttpPut]
		public HttpResponseMessage PutLink([FromBody] UserLinkItem addRec) {

			Logger.Log($"(Add) for {addRec.sectionid}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				UserLinkItem ret = UserLinks.AddLink(addRec);

				Logger.Log($"Add success for section id={addRec.sectionid}");
				httpResponseMessage.Content = new ObjectContent<UserLinkItem>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Put Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}


		// DELETE: /api/UserLinks/User/Link/5)
		[Route("User/Link/{linkid}")]
		[HttpDelete]
		public HttpResponseMessage DeleteLink(int linkid) {

			Logger.Log($"DeleteLink link={linkid} ");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				bool res = UserLinks.DeleteLink(linkid);
				if (res) {
					Logger.Log($"delete success for section id={linkid}");
					httpResponseMessage.Content = new ObjectContent<string>($"Success deleting {linkid}", Configuration.Formatters.JsonFormatter);
					httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
				} else {
					httpResponseMessage = Request.CreateResponse(HttpStatusCode.NoContent);
					Logger.Log($"failed for section id={linkid}");
				}
			} catch (Exception ex) {
				Logger.LogException($"Delete Exception section id={linkid}", ex);
				Logger.Log($"Delete exception for section id={linkid}");
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;

		}

		// POST: /api/UserLinks/Resequence
		[Route("Resequence")]
		[HttpPost]
		public HttpResponseMessage Resequence([FromBody] UserLinksRequenceParams reqParams) {

			//This controller handles resequencing of both section (Master) and UserLinks.  Which type
			//is defined in the params (where the sequence_id is -1 or not).

			Logger.Log($"Start");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				Object ret = UserLinks.Resequence(reqParams);
				httpResponseMessage.Content = new ObjectContent<Object>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
				
			} catch (Exception ex) {
				Logger.LogException($"Resequence Controller failed with exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}
	}
}
