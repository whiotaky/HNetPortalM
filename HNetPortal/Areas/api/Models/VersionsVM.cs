using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSHLib;

namespace HNetPortal.Areas.api.Models {
	public class VersionsVM {
		public AppInfo Portal { get; set; }
		public AppInfo WSHLib { get; set; }
		public AppInfo PFTrack2 { get; set; }
		public AppInfo PFTrack2SoapApi { get; set; }
	}
}