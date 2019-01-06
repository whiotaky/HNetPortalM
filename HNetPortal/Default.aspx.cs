using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WSHLib;

namespace HNetPortal {
	public partial class Default : System.Web.UI.Page {
		protected void Page_Load(object sender, EventArgs e) {
            if (User.Identity.IsAuthenticated) { 
                Logger.Log("/Default.aspx Page_Load: User "+User.Identity.Name + " is logged in; redirecting to /private/default.aspx");
                Response.Redirect("/Private/Default.aspx");
            }

        }
	}
}