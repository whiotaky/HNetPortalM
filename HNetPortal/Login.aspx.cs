using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Text;
using WSHLib;
using WSHLib.Network;



namespace HNetPortal {
	public partial class Login : System.Web.UI.Page {

		protected void Page_Load(object sender, EventArgs e) {

			Logger.Log($"Referer: {Request.UrlReferrer}");

			Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate");

			if (!Page.IsPostBack) {
				string d = this.Request.Params.Get("deauth");
				if (d != null) {
					Logger.Log($"Receieved deauth param from User {User.Identity.Name}.  Logging him out.");
					Global.SignOutUser();					
					return;
				}

			}

			if (User.Identity.IsAuthenticated) {                
                Logger.Log("/Login.aspx Page_Load: User " + User.Identity.Name + " is already logged in; redirecting to /private/default.aspx");
                Response.Redirect("~/Private/Default.aspx");
            }

			//This is to prevent bookmarked and cached login page, because
			//we will typically have an expired session ID which will make 
			//the first good credentials login attempt fail.
			if (Request.UrlReferrer==null) {
				Logger.Log("No referer, redirecting to public landing page");
				Response.Redirect("~/Default.aspx");
			}

        }

        protected void Button1_Click(object sender, EventArgs e) {

			String adPath = "LDAP://hnet.local"; //Fully-qualified Domain Name
            LdapAuthentication adAuth = new LdapAuthentication(adPath);
         
            try {

				var p = Convert.FromBase64String(Password.Text);				
				string sessKey = Crypto.SessionEncKey();
				if (sessKey==null) {
					Logger.Log($"Could not fetch from session EncKey, throwing error");
					throw new Exception("Session Expired, try again");
				}
				Logger.Log($"Session.EncKey={sessKey}");

				var ec = Encoding.UTF8.GetBytes(sessKey);
				if (ec==null) {
					Logger.Log($"Could not decode session key, throwing error");
					throw new Exception("Session Expired, try again");
				}

				string pwd = Crypto.DecryptStringFromBytes(p, ec, ec);

				if (true == adAuth.IsAuthenticated("HNET", UserName.Text, pwd)) {

                    String groups = adAuth.GetGroups();
                    Logger.Log(string.Format("Login: Authenticated! User: {0} Groups: {1}", UserName.Text, groups));

                    if (!groups.Contains("HNet Web Logon") ) {
                        Logger.Log(string.Format("Login: Throwing Group Auth. Error!"));
                        throw new Exception("Group error");
                    }

                    //Create the ticket, and add the groups.
                    bool isCookiePersistent = this.RememberMe.Checked;
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, this.UserName.Text,
                                DateTime.Now, DateTime.Now.AddMinutes(240), isCookiePersistent, groups);

                    //Encrypt the ticket.
                    String encryptedTicket = FormsAuthentication.Encrypt(authTicket);

					//Create a cookie, and then add the encrypted ticket to the cookie as data.
					HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) {
						Domain = FormsAuthentication.CookieDomain,
						Path = "/"
					};


					Logger.Log($"Login: AuthCookie.Domain={authCookie.Domain}");

					if (true == isCookiePersistent)
                        authCookie.Expires = authTicket.Expiration;

                    //Add the cookie to the outgoing cookies collection.
                    Response.Cookies.Add(authCookie);

                    Logger.Log("Login: Athentication complete and successful");

                    //You can redirect now.
                    string rs = FormsAuthentication.GetRedirectUrl(this.UserName.Text, false);
                    Response.Redirect(rs);

                } else {
                    this.FailureText.Text = "Authentication did not succeed. Check user name and password.";
                }

            } catch (System.Threading.ThreadAbortException) { //do there id nothing here
                //this is "not" an error, its a winforms flaw caused
                //bt the redirect
               // this.FailureText.Text = "Not an error " + te.Message;
            } catch (Exception ex) {                
                this.FailureText.Text = "Error authenticating. " + ex.Message;
                Logger.LogException("Login Exception!: " + this.FailureText.Text, ex);
            }

        }
    }
}