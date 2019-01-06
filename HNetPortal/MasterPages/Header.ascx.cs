using MySql.Data.MySqlClient;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using WSHLib;

namespace HNetPortal.MasterPages {
	public partial class Header : ViewUserControl {

	
		protected void Page_Load(object sender, EventArgs e) {
			

			LoginStatus lis = (LoginStatus)LoginView.FindControl("HeadLoginStatus");
			if (lis != null) {
				lis.LogoutText = "<span class='glyphicon glyphicon-user'></span> Logout " + Page.User.Identity.Name;
			}

			PlaceHolder pholder = (PlaceHolder)LoginView.FindControl("menuPlaceHolder");
			if (pholder != null) {
				Logger.Log("Building site menu from database links");
				pholder.Controls.Add(new LiteralControl("<li class='dropdown'>\n"));
				pholder.Controls.Add(new LiteralControl("<a class='dropdown-toggle' href='#' data-toggle='dropdown'><i class='glyphicon glyphicon-bookmark'></i>&nbsp;Site Links<span class='caret'></span></a>\n"));
				pholder.Controls.Add(new LiteralControl("<ul class='dropdown-menu'>\n"));

				int numMenuItems = 0;
				MySqlConnection conn = new MySqlConnection();
				try {

					conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
					conn.Open();

					MySqlCommand cmd = new MySqlCommand("select linkText, linkURL, newwindow from linksdetail where username=@username and  ismenuitem='Y' and enabled='Y' order by orderby", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@username", Page.User.Identity.Name);
					MySqlDataReader reader = cmd.ExecuteReader();

					while (reader.Read()) {
						numMenuItems++;
						string linkText = (string)reader[0];
						string linkURL = (string)reader[1];
						string newWindow = (string)reader[2];
						string targetAttr = "";
						if (newWindow.Equals("Y")) {
							targetAttr = " target='_blank'";
						}
						pholder.Controls.Add(new LiteralControl(string.Format("<li><a href='{0}'{2}><span class='glyphicon glyphicon-link'></span>&nbsp;{1}</a></li>\n", linkURL, linkText, targetAttr)));
					}

				} catch (MySqlException ex) {
					Logger.LogException("DB Exception: ", ex);
				} finally {
					conn.Close();
				}

				if (numMenuItems < 1) {
					Logger.Log("No menu items found; not adding a dropdown to the site menu");
					pholder.Controls.Clear();
				} else {
					pholder.Controls.Add(new LiteralControl("</ul>\n</li>\n"));
					Logger.Log(string.Format("Menu build with {0} items", numMenuItems));
				}

			}
		}
	}
}