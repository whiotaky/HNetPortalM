using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using WSHLib;

namespace HNetPortal {
    

	public static class ICal {
		public class ICalItem {
			public string summary { get; set; }
			public string location { get; set; }
			public string description { get; set; }
			public string startDate { get; set; }
			public string endDate { get; set; }
			public string uid { get; set; }
		}

		public class EventItem {
			public string title { get; set; }
			public string start { get; set; }
			public string allDay { get; set; }
		}

		public static List<ICalItem> ParseFile(string fileName) {

			
			Logger.Log("start for file " + fileName);

			string workDir = ConfigurationManager.AppSettings["WorkDir"];
			string workFileName = workDir + "/" + HttpContext.Current.User.Identity.Name + "_" + fileName;
			
			List<ICalItem> list = new List<ICalItem>();

			try {

				Ical.Net.Calendar calendar = null;
				using (StreamReader sr = new StreamReader(workFileName)) {
					calendar = Ical.Net.Calendar.Load(sr.ReadToEnd());
				}

				//calendar.AddTimeZone(new Ical.Net.VTimeZone("America/New_York"));

				for (int i = 0; i < calendar.Events.Count(); i++) {

					Ical.Net.CalendarComponents.CalendarEvent ev = calendar.Events[i];

					//DTSTART;TZID="America/New_York":20160928T190000
					//2016-09-28 07:00 PM
					//http://www.csharp-examples.net/string-format-datetime/

					ICalItem item = new ICalItem {
						uid = ev.Uid,
						summary = ev.Summary,
						startDate = String.Format("{0:yyyy-MM-dd hh:mm tt}", ev.DtStart.AsSystemLocal),
						endDate = String.Format("{0:yyyy-MM-dd hh:mm tt}", ev.DtEnd != null ? ev.DtEnd.AsSystemLocal : ev.DtStart.AsSystemLocal),
						location = ev.Location,
						description = ev.Description
					};
					list.Add(item);
					// Logger.Log(string.Format("{0}: {1} {2}", item.uid, item.startDate, item.description));
				}
				Logger.Log("Build iCal event list of size " + calendar.Events.Count());

			} catch (System.IndexOutOfRangeException ex) {
				Logger.LogException("ParseFile: OutOfRange Exception (Bad ICal file)", ex);
				throw new Exception("ParseFile: OutOfRange Exception (Bad ICal file): " + ex.Message);
			} catch (Exception ex) {
				Logger.LogException("ParseFile: Exception ", ex);
				throw;
			}

			return list;

		}

	}

}