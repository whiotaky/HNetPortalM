using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HNetPortal.MasterPages {
	public partial class Footer : System.Web.Mvc.ViewUserControl {
		protected string AppVersion { get; set; }
		protected string AppDate { get; set; }
		protected string Copyright { get; set; }
		protected string Author { get; set; }
		protected string WSHLibVersion { get; set; }
		protected string WSHLibDate { get; set; }

		protected void Page_Load(object sender, EventArgs e) {

			AppVersion = Global.PortalInfo.Version;
			AppDate = Global.PortalInfo.Date;
			Copyright = Global.PortalInfo.Copyright;
			Author = Global.PortalInfo.Author;
			WSHLibVersion = WSHLib.Info.AppInfo.Version;
			WSHLibDate = WSHLib.Info.AppInfo.Date;
		}
	}
}
