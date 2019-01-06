using KeePassLib;
using KeePassLib.Collections;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security;
using System.Net;

namespace HNetPortal.Private {
	public partial class PwdVault : System.Web.UI.Page {
		protected void Page_Load(object sender, EventArgs e) {

			string dbpath = (string)ConfigurationManager.AppSettings["KEEPASSPATH"];
			KeePass kp = new KeePass(dbpath, Global.PortalSettings.KeePassDBPassword);
			string s1 = kp.TraverseJS(kp.getRoot());
			this.Literal1.Text = string.Format("[\n{0}\n]", s1);
		
		}
	}
}