using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WSHLib;

namespace HNetPortal.Private {
	public partial class Calendar : System.Web.UI.Page {
		public static string initialDate;
		protected void Page_Load(object sender, EventArgs e) {

			//initialDate = "2013-10-01";
			string mySqlDateFormat = "yyyy-MM-dd";
			DateTime date = DateTime.Today;			
			DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
			initialDate = firstDayOfMonth.ToString(mySqlDateFormat);

			Logger.Log("Starting");

			if (!Page.IsPostBack) {
				string inDateStr = this.Request.Params.Get("initialDate");
				Logger.Log("initialDate Param=" + inDateStr);

				try {
					DateTime tmp = new DateTime(int.Parse(inDateStr.Substring(0,4)), int.Parse(inDateStr.Substring(5,2)),1);
					initialDate = tmp.ToString(mySqlDateFormat);
					Logger.Log($"setting calendar initial date (from param) to {initialDate}");
				} catch(Exception ex) {
					Logger.LogException($"Cant parse date from inDateStr {inDateStr}, defaulting to {initialDate}",ex);
				}

			}

		}
	}
}