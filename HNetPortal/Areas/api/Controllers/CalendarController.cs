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
	[RoutePrefix("api/Calendar")]
	public class CalendarController : ApiController {

		//This route exists because fullcalendar 
		//requires a traditional get with query string params
		[Route("GetEvents/FullCalendar")]
		[HttpGet]
		public List<ICal.EventItem> GetEvents(string start, string end) {

			Logger.Log($"FullCalendar: start-{start}, end-{end}");
			return Calendar.GetEvents(start, end);

		}

		[Route("GetEvents")]
		[HttpPost]
		public List<ICal.EventItem> GetEvents([FromBody]Models.GenericRequest req) {

			Logger.Log($"GetEvents: start-{req.start}, end-{req.end}");
			return Calendar.GetEvents(req.start, req.end);

		}

		//https://stackoverflow.com/questions/10732644/best-practice-to-return-errors-in-asp-net-web-api
		[Route("EditEvent")]
		[HttpPost]
		public HttpResponseMessage EditEvent([FromBody]Models.GenericRequest req) {

			Logger.Log($"EditEvent: calDate-{req.calDate}, calContent num chars-{req.calContent.Length}");
			int result = Calendar.EditEvent(req.calDate, req.calContent);

			if (result != 0) {
				var message = string.Format($"Error updating event {req.calDate}");
				HttpError err = new HttpError(message);
				return Request.CreateResponse(HttpStatusCode.NotFound, err);
			} else {
				return Request.CreateResponse(HttpStatusCode.OK, "Success!");
			}
		}

		[Route("DeleteEvent")]
		[HttpDelete]
		public HttpResponseMessage DeleteEvent([FromBody]Models.GenericRequest req) {

			Logger.Log($"DeleteEvent: calDate-{req.calDate}");
			int result = Calendar.DeleteEvent(req.calDate);

			if (result != 0) {
				var message = string.Format("Error deleting returning Not Found - 204");
				Logger.Log(message);
				HttpError err = new HttpError(message);
				return Request.CreateResponse(HttpStatusCode.NoContent, err);
			} else {				
				return Request.CreateResponse(HttpStatusCode.OK); //deleted okay
			}

		}

		[Route("SearchEvents")]
		[HttpPost]
		public List<ICal.EventItem> SearchEvents([FromBody]Models.GenericRequest req) {

			Logger.Log($"SearchEvents: searchText={req.searchText}");
			return Calendar.SearchEvents(req.searchText);

		}

		//This is an old fashioned query string eg, api/Calendar/UserCalenderHtml?_monthNo=12&_yearNo=1961
		//https://stackoverflow.com/questions/31759979/web-api-2-get-by-query-parameter		 
		[Route("UserCalendarHtml")]
		[HttpGet]
		public HttpResponseMessage UserCalendarHtml(int _monthNo, int _yearNo) {

			Logger.Log($"Get UserCalendarHtml: mo-{_monthNo}, yr-{_yearNo}");

			Stream ms = Calendar.UserCalendarHtml(_monthNo, _yearNo);
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
			httpResponseMessage.Content = new StreamContent(ms);
			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			return httpResponseMessage;

		}

	}
}
