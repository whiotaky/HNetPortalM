using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNetPortal.Areas.api.Models {

	//add to as needed!
	public class GenericRequest {

		//Multiuse
		public bool allowCached { get; set; }

		//For Quotes
		public string symbols { get; set; }

		//For Logs
		public string whichLog { get; set; }
		public int numLines { get; set; }

		//For TFS
		public string sourcePathName { get; set; }
		public string sourceFileName { get; set; }

		//For Calendar
		public string start { get; set; }
		public string end { get; set; }
		public string calDate { get; set; }
		public string calContent { get; set; }
		public string searchText { get; set; }

	}
}