using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc.Html;


namespace HNetPortal {
	public partial class RazorView : System.Web.Mvc.ViewPage<dynamic> {
		protected void Page_Load(object sender, EventArgs e) {
			//Html.RenderPartial("");
		}

		public void RenderPartial(string name) {
			Html.RenderPartial(name);
		}
	}
}