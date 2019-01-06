using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using WSHLib;


namespace HNetPortal.Private {

	public partial class Default : System.Web.UI.Page {

		private MySqlConnection conn;
		//Boolean clientIsIntranet = false;

		protected void Page_Load(object sender, EventArgs e) {

			Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate");

			try {
				conn = new MySql.Data.MySqlClient.MySqlConnection {
					ConnectionString = Global.getConnString(SupportedDBTypes.MySql)
				};
				conn.Open();
				renderColumn2();
				renderColumn2_Careers();
				renderColumn3();
			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Logger.LogException("getCache()", ex);
			} finally {
				conn.Close();
			}
		
		}
		
		private void renderColumn2() {

			Logger.Log("renderUserColumn start");

			List<string> sectionList = new List<string>();

			try {				
				MySqlCommand cmd = new MySqlCommand("select sectionid, sectionText, orderby from linkssection where username=@username and enabled='Y' order by orderby;", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);				
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				
				while (reader.Read()) {				
					sectionList.Add(String.Format("{0}|{1}", reader[0], reader[1]));
				}
				reader.Close();

			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Logger.LogException("renderUserColumn() EXCEPTION", ex);
			} 

			foreach (string sec in sectionList) {
				string[] arr = sec.Split('|');
				Logger.Log("SECTION="+arr[1]);
				int feedId = int.Parse(arr[0]);
				renderUserSection(feedId, arr[1]);				
			}


		}


		private void renderColumn2_Careers() {
			int plNo = 2501;
			Logger.Log("start");
			renderNewsFeed(2, column2, ref plNo);
		}


		private void renderUserSection (int sectionid, string sectionText) {
		
			string userLinks = "";

			try {

				MySqlCommand cmd = new MySqlCommand("select linkText,linkURL,hoverText,subSectionText, newwindow from linksdetail where username=@username and sectionid=@sectionId and enabled='Y' order by orderby;", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.Parameters.AddWithValue("@sectionId", sectionid);
				MySqlDataReader reader;

				string currSS = "";
				int ssOn = 0;
				reader = cmd.ExecuteReader();
				while (reader.Read()) {
					userLinks = userLinks + renderUserLinks((string)reader[0],(string) reader[1],(string) reader[2], (string) reader[3], (string)reader[4], ref currSS, ref ssOn);
				}
				reader.Close();

				HtmlGenericControl headDiv = new HtmlGenericControl("div");
				headDiv.Attributes.Add("class", "panel-heading");
				headDiv.Attributes.Add("id", sectionid.ToString());
				headDiv.InnerHtml = "<h3 class=\"panel-title\"> <span class=\"glyphicon glyphicon-bookmark\"></span>&nbsp;&nbsp;&nbsp;" + sectionText + "</h3>";

				HtmlGenericControl bodyDiv = new HtmlGenericControl("div");
				bodyDiv.Attributes.Add("class", "panel-body");
				bodyDiv.InnerHtml = "<ul>"+userLinks+"</ul>";

				HtmlGenericControl panelDiv = new HtmlGenericControl("div");
				panelDiv.Attributes.Add("class", "panel panel-success triPanel");
				//panelDiv.InnerHtml = "HI";
				panelDiv.Controls.Add(headDiv);
				panelDiv.Controls.Add(bodyDiv);

				column2.Controls.Add(panelDiv);

			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Logger.LogException("renderUserSection() EXCEPTION", ex);
			}

		}


		private string renderUserLinks(string linkText, string linkURL, string hoverText, string subSectionText, string newWindow, ref string currSS,   ref int ssOn) {

			string titleAttr = "";
			if (!hoverText.Equals("")) {
				titleAttr = string.Format("title='{0}'", hoverText);
			}

            string targetAttr = "";
            if (newWindow.Equals("Y")) {
                targetAttr = "target='_blank'";
            }

            string html_url = String.Format("<h3>{0}</h3>", linkText);
			if (!linkURL.Equals("")) {
				html_url = String.Format("<a href='{0}' {1} {3}>{2}</a>", linkURL, titleAttr, linkText, targetAttr);
			}

			String s = "";
			if (currSS.Equals(subSectionText)) {
				s = String.Format("<li>{0}</li>\n", html_url);
			} else {

				if (ssOn == 1) {
					s = "</ul>\n";
				}

				s = String.Format("{0}<li>{1}</li>\n", s, html_url);

				if (subSectionText.Equals("")) {
					ssOn = 0;
				} else {
					s = String.Format("{0}<ul class='inner'>\n", s);
					ssOn = 1;
				}
				currSS = subSectionText;
			}

			return s;

        }


		public void renderColumn3() {
			int plNo = 3001;
			Logger.Log("start");
			renderNewsFeed(1, column3, ref plNo);
		}


		//<div id="postload_$postLoadNext"  data-url='?AJAX=loadfeed&feedid=$feedid'>
		public void renderNewsFeed(int xfeedType, HtmlGenericControl column, ref int plNo ) {

            Logger.Log("start");

			try {
				MySqlCommand cmd = new MySqlCommand("select a.feedid, b.feedname,  a.orderby, b.feedtype from userfeedprefs a, feedsmaster b where b.feedid = a .feedid and b.feedtype=@feedType and a.username=@username and b.enabled='Y' order by b.feedtype, a.orderby;", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.Parameters.AddWithValue("@feedType", xfeedType.ToString());
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();

				while (reader.Read()) {
					int feedId = (int)reader[0];				
					string feedName = (string)reader[1];
					int feedType = Convert.ToInt32(reader[3]);

					HtmlGenericControl headDiv = new HtmlGenericControl("div");
					headDiv.Attributes.Add("class", "panel-heading");
					headDiv.Attributes.Add("id", feedId.ToString());
					headDiv.InnerHtml = "<h3 class=\"panel-title\"> <span class=\"glyphicon glyphicon-bookmark\"></span>&nbsp;&nbsp;&nbsp;" + feedName + "</h3>";

					HtmlGenericControl bodyDiv = new HtmlGenericControl("div");
					bodyDiv.Attributes.Add("class", "panel-body triPanel");
					bodyDiv.Attributes.Add("id", String.Format("postload_{0}", plNo++));
					bodyDiv.Attributes.Add("data-feedid", feedId.ToString());
					string imgUrl = ResolveClientUrl("~/images/ajax-loader-feed2.gif");
					bodyDiv.InnerHtml = "<div class='postload_placeholder'><img src='" + imgUrl + "' width='34' height='34'></div>";

					HtmlGenericControl panelDiv = new HtmlGenericControl("div");
					panelDiv.Attributes.Add("class", "panel panel-success");
					panelDiv.Controls.Add(headDiv);
					panelDiv.Controls.Add(bodyDiv);

					column.Controls.Add(panelDiv);
				}

				reader.Close();

			} catch (MySql.Data.MySqlClient.MySqlException ex) {
				Logger.LogException("renderNewsFeed EXCEPTION", ex);
			}

		}

	}
}