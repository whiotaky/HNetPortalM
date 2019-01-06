using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using WSHLib;

namespace HNetPortal {
	public static class Calendar {

		public static List<ICal.EventItem>  GetEvents(string start, string end) {
			
			Logger.Log(string.Format("from={0} to={1}", start, end));

			List<ICal.EventItem> list = new List<ICal.EventItem>();
			var jsonSerialiser = new JavaScriptSerializer();
			MySqlConnection conn = new MySqlConnection();
			try {
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("select  date_format(calDate, '%Y-%m-%d')  as eventDate, content from calendar where username=@username and (calDate >=@fromDate  and calDate <=@toDate) order by calDate", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@fromDate", start);
				cmd.Parameters.AddWithValue("@toDate", end);

				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				while (reader.Read()) {
					ICal.EventItem eitem = new ICal.EventItem {
						start = (string)reader[0],
						title = (string)reader[1],
						allDay = "true"
					};
					list.Add(eitem);
				}

			} catch (MySqlException ex) {
				Logger.LogException("GetEvents() Exception: ", ex);

			} finally {
				conn.Close();
			}

			Logger.Log("Ending with return list");
			return list;// jsonSerialiser.Serialize(list);

		}

		public static List<ICal.EventItem> SearchEvents(string searchText) {

			Logger.Log($"SearchText={searchText}");

			List<ICal.EventItem> list = new List<ICal.EventItem>();
			var jsonSerialiser = new JavaScriptSerializer();
			MySqlConnection conn = new MySqlConnection();
			try {
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("select  date_format(calDate, '%Y-%m-%d')  as eventDate, content from calendar where username=@username and content like @searchText order by calDate", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@searchText", $"%{searchText}%");				

				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				while (reader.Read()) {
					ICal.EventItem eitem = new ICal.EventItem {
						start = (string)reader[0],
						title = (string)reader[1],
						allDay = "true"
					};
					list.Add(eitem);
				}

			} catch (MySqlException ex) {
				Logger.LogException("SearchEvents() Exception: ", ex);

			} finally {
				conn.Close();
			}

			Logger.Log("Ending with return list");
			return list;// jsonSerialiser.Serialize(list);

		}

		public static int EditEvent(string calDate, string calContent) {

			//This should be restful in its response.  ie, 
			//return 200 if it worked, and non-200 if it did not.
			//https://restfulapi.net/http-status-codes/

			Logger.Log(string.Format("calDate={0} calContent(length)={1}", calDate, calContent.Length));

			Logger.Log("start for user " + HttpContext.Current.User.Identity.Name);
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("replace into calendar set username=@username, caldate=@caldate, content=@content", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@caldate", calDate);
				cmd.Parameters.AddWithValue("@content", calContent);
				cmd.ExecuteNonQuery();

			} catch (MySqlException ex) {
				Logger.LogException("getCalEvent() Exception: ", ex);
				return 1;
			} finally {
				conn.Close();
			}
			Logger.Log("End");
			return 0;

		}

		public static int DeleteEvent(string calDate) {

			Logger.Log(string.Format("calDate={0} (Ignoring content)", calDate));

			Logger.Log("start for user " + HttpContext.Current.User.Identity.Name);
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("delete from calendar where username=@username and caldate=@caldate", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@caldate", calDate);

				cmd.ExecuteNonQuery();

			} catch (MySqlException ex) {
				Logger.LogException("deleteCalEvent() Exception: ", ex);
				return 1;
			} finally {
				conn.Close();
			}
			Logger.Log("End");
			return 0;

		}

		public static Stream UserCalendarHtml(int _monthNo, int _yearNo) {

			Logger.Log($"Start monthNo={_monthNo}, yearNo={_yearNo}");

			string mySqlDateFormat = "yyyy-MM-dd";

			DateTime rDate = DateTime.Today;
			DateTime todaysDate = DateTime.Today;

			//these are a constant; today's month and year
			int realMonth = rDate.Month;
			int realYear = rDate.Year;

			int monthNo = (_monthNo > 0 && _monthNo < 13) ? _monthNo : rDate.Month;
			int yearNo = (_yearNo > 2000) ? _yearNo : rDate.Year;
			//prevent setting invalid day number eg, feb 30th, april 31st, etc.
			int todayDay = DateTime.DaysInMonth(yearNo, monthNo) < rDate.Day ? DateTime.DaysInMonth(yearNo, monthNo) : rDate.Day;


			rDate = new DateTime(yearNo, monthNo, todayDay);
			string monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(rDate.Month);

			DateTime firstDayOfMonth = new DateTime(yearNo, monthNo, 1);
			DateTime LastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

			DateTime lastMonth = rDate.AddMonths(-1);
			DateTime nextMonth = rDate.AddMonths(+1);

			Logger.Log($"calendar's date is {monthNo}/{todayDay}/{yearNo}");

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append($@"
						<table id='calTable' class='table-condensed table-bordered table-striped'>
						<thead>							
							<tr>
								<th onclick='loadCalendar({lastMonth.Month},{lastMonth.Year})' class='navbar-default' style='color:white; cursor: pointer; text-align:center'><i class='glyphicon glyphicon-backward'></i></th>
								<th onclick='launchCalSearch();' class='navbar-default' style='color:white; cursor: pointer; text-align:center'><i class='glyphicon glyphicon-search'></i></th>
								<th colspan='3' style='width: 250px;text-align:center'><a href='{System.Web.VirtualPathUtility.ToAbsolute("~/Private/Calendar.aspx")}?InitialDate={firstDayOfMonth.ToString(mySqlDateFormat)}'>{monthName} {rDate.Year} </a></th>
								<th onclick='loadCalendar({realMonth},{realYear})' class='navbar-default' style='color:white; cursor: pointer; text-align:center'><i class='glyphicon glyphicon-home'></i></th>
								<th onclick='loadCalendar({nextMonth.Month},{nextMonth.Year})' class='navbar-default' style='color:white;  cursor: pointer; text-align:center'><i class='glyphicon glyphicon-forward'></i></th>
							</tr>
							<tr>
								<th>Su</th><th>Mo</th><th>Tu</th><th>We</th><th>Th</th><th>Fr</th><th>Sa</th>
							</tr>
						</thead>
						<tbody>
						<tr>
			");

			List<ICal.EventItem> eventList = GetEvents(firstDayOfMonth.ToString(mySqlDateFormat), LastDayOfMonth.ToString(mySqlDateFormat));
			//string eventsJson = GetEvents(firstDayOfMonth.ToString(mySqlDateFormat), LastDayOfMonth.ToString(mySqlDateFormat));
			//List<EventItem> eventList = JsonConvert.DeserializeObject<List<EventItem>>(eventsJson);
			List<string> cells = new List<string>(42);
			int currDayNo = 1;
			for (int i = 0; i < 42; i++) {

				int dayOfWeekIdx = i % 7;
				try {

					DateTime currDay = new DateTime(rDate.Year, rDate.Month, currDayNo);
					string @class = "class='calCell'";

					if ((int)currDay.DayOfWeek == dayOfWeekIdx) {

						string hover = "";
						ICal.EventItem todaysEvent = eventList.FirstOrDefault(x => x.start.Equals(currDay.ToString(mySqlDateFormat)));
						if (todaysEvent != null) {
							//Logger.Log($"event found for {todaysEvent.start}: {todaysEvent.title}");							
							if (currDay == todaysDate)
								@class = "class='calCell calCellHighlight calCellToday'";
							else
								@class = "class='calCell calCellHighlight'";

							hover = $"title='{todaysEvent.title.Replace("\'", "\\'")}'";
						} else {
							if (currDay == todaysDate)
								@class = "class='calCell calCellHighlight calCellToday'";
						}

						builder.AppendLine($"<td  class='calCell' onclick=' launchCalEditDlg(\"{currDay.ToString(mySqlDateFormat)}\");'><a {@class} href='#' {hover}>{currDayNo}</a></td>");
						currDayNo++;

					} else {
						builder.AppendLine($"<td class='muted'>&nbsp;</td>");
					}

				} catch (Exception) {
					builder.AppendLine($"<td class='muted'>&nbsp;</td>");
				}

				if (dayOfWeekIdx == 6) {
					builder.AppendLine("</tr><tr>");
				}
			}
			builder.AppendLine($@"</tr></tbody></table>");

			Logger.Log("Ending");
			byte[] resultBytes = Encoding.UTF8.GetBytes(builder.ToString());
			//WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
			//return new MemoryStream(resultBytes);

			return new MemoryStream(resultBytes);

		}

	}
}