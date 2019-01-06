using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WSHLib;

namespace HNetPortal.Private {
    public partial class knockoutTester : System.Web.UI.Page {

		public string searchTxt = "";
		protected void Page_Load(object sender, EventArgs e) {

            Logger.Log("Starting");

			if (!Page.IsPostBack) {

				searchTxt = this.Request.Params.Get("searchTxt");
				Logger.Log("search Param="+searchTxt);
				
			}

		}
    }
}