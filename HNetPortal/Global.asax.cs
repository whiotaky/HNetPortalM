using System;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using System.Configuration;
using System.Xml;
using System.Security;
using System.Web.Mvc;
using System.Web.Http;
using System.Diagnostics;
using System.Reflection;
using System.Web.Configuration;
using WSHLib;

namespace HNetPortal {

	public enum SupportedDBTypes {
		MySql = 0,
		MSSql
	}

	
	//This class is for settings that are too sensitive to store
	//in web.config.  The settings are stored in an XML file outside
	//of the project and read in in application_startup.
	public class PortalSettings {
		public string MySqlUserName { get; set; }
		public string MySqlPassword { get; set; }
		public SecureString KeePassDBPassword { get; set; }
		public string KeePassEncrptionKey { get; set; }
		public string KeePassEncrptionIv { get; set; }
	
	}	

	public class Global : System.Web.HttpApplication {
		public static PortalSettings PortalSettings;
		public static AppInfo PortalInfo;

		protected void Application_Start(object sender, EventArgs e) {

			//For WebApi. Keep it before AreaRegistration
			GlobalConfiguration.Configure(WebApiConfig.Register);

			//For Areas section
			AreaRegistration.RegisterAllAreas();

			//Get app version, date, etc
			Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			object dateAttr = assembly.GetCustomAttributes(typeof(CustomAssemblyAppDate), false)[0];
			object verAttr = assembly.GetCustomAttributes(typeof(CustomAssemblyAppVersion), false)[0];
			object authorAttr = assembly.GetCustomAttributes(typeof(CustomAssemblyAuthor), false)[0];
			object[] crAttr = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			object[] titleAttr = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
			object[] compAttr = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			object[] prodAttr = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

			PortalInfo = new AppInfo {
				Version = ((CustomAssemblyAppVersion)verAttr).Version,
				Date = ((CustomAssemblyAppDate)dateAttr).Date,
				Author = ((CustomAssemblyAuthor)authorAttr).Author,
				Copyright = ((AssemblyCopyrightAttribute)crAttr[0]).Copyright,
				Title = ((AssemblyTitleAttribute)titleAttr[0]).Title,
				Company = ((AssemblyCompanyAttribute)compAttr[0]).Company,
				Product = ((AssemblyProductAttribute)prodAttr[0]).Product
			};

			PortalSettings = new PortalSettings {
				MySqlUserName = string.Empty,
				MySqlPassword = string.Empty,
				KeePassEncrptionKey = string.Empty,
				KeePassEncrptionIv = string.Empty
			};

			Logger.Log($"Initializing {PortalInfo.Title} version {PortalInfo.Version} ({PortalInfo.Date})");
			string filename = ConfigurationManager.AppSettings["WorkDir"] + "/HnetPortalSettings.xml";

			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				Logger.Log("Loading settings from XML");
				foreach (XmlNode node in doc.DocumentElement.ChildNodes) {

					switch (node.Name) {
						case "MySqlAccount":
							PortalSettings.MySqlUserName = node.SelectSingleNode("UserName").InnerText;
							PortalSettings.MySqlPassword = node.SelectSingleNode("Password").InnerText;
							break;
						case "KeePass":
							PortalSettings.KeePassDBPassword = Crypto.MakeSecureString(node.SelectSingleNode("DatabasePassword").InnerText);
							XmlNode encryptionNode = node.SelectSingleNode("Encryption");
							PortalSettings.KeePassEncrptionKey = encryptionNode.SelectSingleNode("Key").InnerText;
							PortalSettings.KeePassEncrptionIv = encryptionNode.SelectSingleNode("Iv").InnerText;
							break;
						default:
							break;

					}
				}
			} catch (Exception ex) {
				Logger.LogException($"Error loading {filename}: ", ex);
			}
		}

		/// <summary>
		/// The HNet purpose of this code is to load the user's AD role
		/// so we can use it for page-level access restrictions, based on
		/// Administrators vs Users, etc.  It serves no other site purpose.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_AuthenticateRequest(Object sender, EventArgs e) {

			String cookieName = FormsAuthentication.FormsCookieName;
			HttpCookie authCookie = Context.Request.Cookies[cookieName];

			if (null == authCookie) {//There is no authentication cookie.
				Logger.Log("No cookie! returning.");
				return;
			}

			FormsAuthenticationTicket authTicket = null;

			try {
				authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			} catch (Exception ex) {
				//Write the exception to the Event Log.
				Logger.Log("Error Decrypting cookie: " + ex);
				return;
			}

			if (null == authTicket) {//Cookie failed to decrypt.
				Logger.Log("Global_Auth: Error Decryping cookie: null cookie!");
				return;
			}

			//When the ticket was created, the UserData property was assigned a
			//pipe-delimited string of group names.
			String[] groups = authTicket.UserData.Split(new char[] { '|' });

			//Create an Identity.
			GenericIdentity id = new GenericIdentity(authTicket.Name, "LdapAuthentication");

			//This principal flows throughout the request.
			GenericPrincipal principal = new GenericPrincipal(id, groups);

			Context.User = principal;
			Logger.Log("Request is authenticated, user name=" + Context.User.Identity.Name);
		}

		public static string getConnString(SupportedDBTypes dbType, string dbName = null) {

			Logger.Log(string.Format("Conn string for type {0} requested", dbType.ToString()));
			string ret = "";
			try {
				if (dbType == SupportedDBTypes.MySql) {

					ret = ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString;
					ret = ret.Replace("{userid}", PortalSettings.MySqlUserName);
					ret = ret.Replace("{password}", PortalSettings.MySqlPassword);

				} else if (dbType == SupportedDBTypes.MSSql) {

					if (dbName == null) {
						throw new Exception("No DBName supplied for MSSqlDB");
					}

					ret = ConfigurationManager.ConnectionStrings["MSSqlConnString"].ConnectionString;
					ret = ret.Replace("{catalog}", dbName);
				}

				Logger.Log(string.Format("Built a conn string for {0}", dbType.ToString()));
				return ret;

			} catch (Exception ex) {
				Logger.LogException("Error", ex);
				throw new Exception("Exception Building DB Connection string!");
			}

		}

		public static void SignOutUser() {

			string redirUrl = "~/Default.aspx";
			Logger.Log($"Global.SignOutUser will signout and redirect to {redirUrl}");
			try {
				HttpContext.Current.Server.ClearError();
				HttpContext.Current.Session.Clear();
				HttpContext.Current.Session.Abandon();
				HttpContext.Current.Session.RemoveAll();

				FormsAuthentication.SignOut();
				//Roles.DeleteCookie(); Not implemented on the HNetPortal

				// clear authentication cookie
				HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
				cookie1.Expires = DateTime.Now.AddYears(-1);
				HttpContext.Current.Response.Cookies.Add(cookie1);

				// clear session cookie 
				SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
				HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
				cookie2.Expires = DateTime.Now.AddYears(-1);
				HttpContext.Current.Response.Cookies.Add(cookie2);

				HttpContext.Current.Response.Redirect(redirUrl);
			} catch(Exception ex) {
				Logger.LogException($"signout exception, but as long as the redirect happened we are good", ex);				
			}
			Logger.Log("end");

		}
		
	}
}