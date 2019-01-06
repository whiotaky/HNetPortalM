using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Script.Serialization;
using WSHLib;

namespace HNetPortal.Areas.api.Controllers {

	[HNetAuthorize]
	public class ShippingTrackController : ApiController {

		// GET: api/ShippingTrack/{shipperCode}
		[HttpGet]
		[Route("api/ShippingTrack/{shipperCode}")]
		public HttpResponseMessage Get(string shipperCode) {

			Logger.Log($"ShippingTrack Get() for {shipperCode}");

			List<ShipperTrack.ShipperTrackItem> shipments = ShipperTrack.ShipperTrackGetList(shipperCode);
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
			httpResponseMessage.Content = new ObjectContent<List<ShipperTrack.ShipperTrackItem>>(shipments, Configuration.Formatters.JsonFormatter);
			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

			return httpResponseMessage;

		}

		// POST api/<controller>	
		public HttpResponseMessage Post([FromBody]ShipperTrack.ShipperTrackItem req) {

			Logger.Log($"POST api/ShippingTrack  Shipper={req.shipperCode}, tracking={req.trackingNo}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
			
			try {
				ShipperTrack.ShipperTrackInsert(req.trackingNo, req.shipperCode);

				httpResponseMessage.Content = new ObjectContent<string>("Success", Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Exception in ShippingTrack controller", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;

		}

	}
}
