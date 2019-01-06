using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using WSHLib;

namespace HNetPortal {

	public class UserLinkSecItem {
		public int sectionid { get; set; }
		public int orderby { get; set; }
		public string sectionText { get; set; }
		public string enabled { get; set; }
		public string timestamp { get; set; }
	}

	public class UserLinkItem {
		public int linkid { get; set; }
		public int sectionid { get; set; }
		public int orderby { get; set; }
		public string linkText { get; set; }
		public string linkURL { get; set; }
		public string subSectionText { get; set; }
		public string enabled { get; set; }
		public string hoverText { get; set; }
		public string timestamp { get; set; }
		public string newwindow { get; set; }
		public string ismenuitem { get; set; }
	}

	public class UserLinksRequenceParams {
		public string procName { get; set; }
		public int seq_sectionId { get; set; }
		public int seq_initialNum { get; set; }
		public int seq_incrementBy { get; set; }
	}

	public static class UserLinks {

		public static UserLinkSecItem GetSection(int sectionid) {

			Logger.Log($"GetSection id {sectionid}");
			try {
				List<UserLinkSecItem> ret = GetSectionsInternal(sectionid);
				return ret[0];
			} catch(Exception ex)  {
				Logger.LogException($"GetSection id {sectionid} exception", ex);
				throw;
			}
		}
		public static  List<UserLinkSecItem> GetSections() {

			Logger.Log($"GetSections");
			try {
				return GetSectionsInternal();
			} catch (Exception ex) {
				Logger.LogException($"GetSections (List) exception", ex);
				throw;
			}
		}

		private static  List<UserLinkSecItem> GetSectionsInternal(int sectionid = 0) {


			Logger.Log($"start for sectionID {sectionid} User {HttpContext.Current.User.Identity.Name}");

			MySqlConnection conn = new MySqlConnection();
			List<UserLinkSecItem> list = new List<UserLinkSecItem>();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd;
				if (sectionid <= 0) {
					cmd = new MySqlCommand("select sectionid, orderby, sectionText,enabled, DATE_FORMAT(timestamp, '%m-%d-%Y %h:%i:%s') as timestamp from linkssection where username=@username order by orderby", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				} else {
					cmd = new MySqlCommand("select sectionid, orderby, sectionText,enabled, DATE_FORMAT(timestamp, '%m-%d-%Y %h:%i:%s') as timestamp from linkssection where username=@username and sectionid=@sectionid order by orderby", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
					cmd.Parameters.AddWithValue("@sectionid", sectionid);
				}
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				while (reader.Read()) {
					UserLinkSecItem litem = new UserLinkSecItem {
						sectionid = (int)reader[0],
						orderby = (int)reader[1],
						sectionText = (string)reader[2],
						enabled = (string)reader[3],
						timestamp = (string)reader[4]
					};
					list.Add(litem);
				}

			} catch (Exception ex) {
				Logger.LogException("_GetSections: ", ex);
				throw;

			} finally {
				conn.Close();
			}

			Logger.Log("end");
			return list;
		}

		public static bool DeleteSection(int sectionid) {

			Logger.Log($"start for sectionID {sectionid} User {HttpContext.Current.User.Identity.Name}");

			MySqlConnection conn = new MySqlConnection();
			try {
				Logger.Log("DeleteSection(" + sectionid.ToString() + ") deleting Section record  record for user " + HttpContext.Current.User.Identity.Name);
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("delete from linkssection where username=@username and sectionid=@sectionid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@sectionid", sectionid);
				cmd.ExecuteNonQuery();

				Logger.Log("DeleteSection(" + sectionid.ToString() + "), deleting links for sectionid " + sectionid.ToString() + " User=" + HttpContext.Current.User.Identity.Name);
				cmd = new MySqlCommand("delete from linksdetail where username=@username and sectionid=@sectionid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@sectionid", sectionid);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException($"DeleteSection( {sectionid}  User= {HttpContext.Current.User.Identity.Name} )", ex);
				return false;

			} finally {
				conn.Close();
			}

			Logger.Log($"DeleteSection({sectionid} User={HttpContext.Current.User.Identity.Name}): Finished");
			return true;

		}

		public static UserLinkSecItem UpdateSection(UserLinkSecItem updSec) {
		
			Logger.Log($"start for sectionID {updSec.sectionid} User {HttpContext.Current.User.Identity.Name}");

			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("Update linkssection set sectionText=@sectionText, orderby=@orderby, enabled=@enabled, username=@username, timestamp=CURRENT_TIMESTAMP where sectionid=@sectionid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@sectionid", updSec.sectionid);
				cmd.Parameters.AddWithValue("@sectionText", updSec.sectionText);
				cmd.Parameters.AddWithValue("@orderby", updSec.orderby);
				cmd.Parameters.AddWithValue("@enabled", updSec.enabled);
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException($"({updSec.sectionid} User={HttpContext.Current.User.Identity.Name})", ex);
				throw;

			} finally {
				conn.Close();
			}

			Logger.Log($"UpdateSection ({updSec.sectionid} User={HttpContext.Current.User.Identity.Name}): Finished");
			return GetSection(updSec.sectionid);

		}

		public static UserLinkSecItem AddSection (UserLinkSecItem addSec) {

			Logger.Log($"start for sectionID {addSec.sectionid} User {HttpContext.Current.User.Identity.Name}");

			int lastId = 0;
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("insert into linkssection(username, orderby, sectionText, enabled, timestamp) values(@username,@orderby, @sectionText, @enabled, CURRENT_TIMESTAMP)", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@sectionText", addSec.sectionText);
				cmd.Parameters.AddWithValue("@orderby", addSec.orderby);
				cmd.Parameters.AddWithValue("@enabled", addSec.enabled);
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.ExecuteNonQuery();

				cmd = new MySqlCommand("select max(sectionid) as lastId from linkssection where username=@username", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				reader.Read();
				lastId = (int)reader[0];
				Logger.Log("The new record's sectionid " + lastId.ToString());
				reader.Close();

			} catch (Exception ex) {
				Logger.LogException($"({addSec.sectionid} User={HttpContext.Current.User.Identity.Name})", ex);
				throw;

			} finally {
				conn.Close();
			}

			Logger.Log($"({addSec.sectionid} User={HttpContext.Current.User.Identity.Name}) : Finished");
			return GetSection(lastId);

		}

		public static UserLinkItem GetLink(int sectionid, int linkid) {

			Logger.Log($"GetLink section= {sectionid} link={linkid}");
			try {
				List<UserLinkItem> ret = GetLinksInternal(sectionid, linkid);
				return ret[0];
			} catch (Exception ex) {
				Logger.LogException($"GetLink section={sectionid} link={linkid} exception", ex);
				throw;
			}
		}
		public static List<UserLinkItem> GetLinks(int sectionid) {

			Logger.Log($"GetLinks section={sectionid}");

			try {
				return GetLinksInternal(sectionid);
			} catch (Exception ex) {
				Logger.LogException($"GetLinks (List) exception", ex);
				throw;
			}
		}


		private static List<UserLinkItem> GetLinksInternal(int sectionid, int linkid=0) {
			
			Logger.Log("start");
			MySqlConnection conn = new MySqlConnection();			
			List<UserLinkItem> list = new List<UserLinkItem>();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd;
				if (linkid <= 0) {
					cmd = new MySqlCommand("select linkid, sectionid, orderby, linkText,linkURL, subSectionText, hoverText, enabled, DATE_FORMAT(timestamp, '%m-%d-%Y %h:%i:%s') as timestamp, newwindow, ismenuitem from linksdetail where username=@username and sectionid=@sectionid order by orderby", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@sectionid", sectionid);
					cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				} else {
					cmd = new MySqlCommand("select linkid, sectionid, orderby, linkText,linkURL, subSectionText, hoverText, enabled, DATE_FORMAT(timestamp, '%m-%d-%Y %h:%i:%s') as timestamp, newwindow, ismenuitem from linksdetail where username=@username and  linkid=@linkid order by orderby", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
					cmd.Parameters.AddWithValue("@linkid", linkid);
				}

				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				while (reader.Read()) {
					UserLinkItem litem = new UserLinkItem {
						linkid = (int)reader[0],
						sectionid = (int)reader[1],
						orderby = (int)reader[2],
						linkText = (string)reader[3],
						linkURL = (string)reader[4],
						subSectionText = (string)reader[5],
						hoverText = (string)reader[6],
						enabled = (string)reader[7],
						timestamp = (string)reader[8],
						newwindow = (string)reader[9],
						ismenuitem = (string)reader[10]
					};
					list.Add(litem);
				}

			} catch (Exception ex) {
				Logger.LogException("GetLinks: ", ex);
				throw;

			} finally {
				conn.Close();
			}

			Logger.Log("end, returning a list of size: " + list.Count());		
			return list;
		}

		public static UserLinkItem UpdateLink(UserLinkItem updSec) {

			Logger.Log($"start for sectionID {updSec.sectionid} linkid {updSec.linkid} User {HttpContext.Current.User.Identity.Name}");

			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("update linksdetail set sectionid=@sectionid, orderby=@orderby, username=@username, linkText=@linkText,linkURL=@linkURL, subSectionText=@subSectiontext, hoverText=@hoverText, enabled=@enabled, ismenuitem=@isMenuItem, timestamp=CURRENT_TIMESTAMP, newwindow=@newWindow where linkid=@linkid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@linkid", updSec.linkid);
				cmd.Parameters.AddWithValue("@sectionid", updSec.sectionid);
				cmd.Parameters.AddWithValue("@linkText", updSec.linkText);
				cmd.Parameters.AddWithValue("@linkURL", updSec.linkURL);
				cmd.Parameters.AddWithValue("@subSectionText", updSec.subSectionText);
				cmd.Parameters.AddWithValue("@hoverText", updSec.hoverText);
				cmd.Parameters.AddWithValue("@orderby", updSec.orderby);
				cmd.Parameters.AddWithValue("@enabled", updSec.enabled);
				cmd.Parameters.AddWithValue("@newWindow", updSec.newwindow);
				cmd.Parameters.AddWithValue("@isMenuItem", updSec.ismenuitem);
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.ExecuteNonQuery();


			} catch (Exception ex) {
				Logger.LogException($"({updSec.sectionid}  linkid {updSec.linkid}  User={HttpContext.Current.User.Identity.Name})", ex);
				throw;

			} finally {
				conn.Close();
			}

			Logger.Log($"UpdateSection ({updSec.sectionid} User={HttpContext.Current.User.Identity.Name}): Finished");
			return GetLink(updSec.sectionid, updSec.linkid);

		}

		public static UserLinkItem AddLink(UserLinkItem addSec) {

			Logger.Log($"start for sectionID {addSec.sectionid}  User {HttpContext.Current.User.Identity.Name}");


			int lastId = 0;
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("insert into linksdetail( sectionid, orderby, username, linkText,linkURL, subSectionText, hoverText, enabled,newWindow, ismenuitem, timestamp) values(@sectionid, @orderby, @username, @linkText,@linkURL, @subSectionText, @hoverText, @enabled, @newWindow, @isMenuItem, CURRENT_TIMESTAMP)", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@sectionid", addSec.sectionid);
				cmd.Parameters.AddWithValue("@linkText", addSec.linkText);
				cmd.Parameters.AddWithValue("@linkURL", addSec.linkURL);
				cmd.Parameters.AddWithValue("@subSectionText", addSec.subSectionText);
				cmd.Parameters.AddWithValue("@hoverText", addSec.hoverText);
				cmd.Parameters.AddWithValue("@orderby", addSec.orderby);
				cmd.Parameters.AddWithValue("@enabled", addSec.enabled);
				cmd.Parameters.AddWithValue("@newWindow", addSec.newwindow);
				cmd.Parameters.AddWithValue("@isMenuItem", addSec.ismenuitem);
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.ExecuteNonQuery();

				cmd = new MySqlCommand("select max(linkid) as lastId from linksdetail where username=@username", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				reader.Read();
				lastId = (int)reader[0];
				Logger.Log("The new record's linkid is " + lastId.ToString());
				reader.Close();


			} catch (Exception ex) {
				Logger.LogException($"AddLink({addSec.linkid} User={HttpContext.Current.User.Identity.Name} )", ex);
				throw;
			} finally {
				conn.Close();
			}

			Logger.Log($"AddLink {lastId} User={HttpContext.Current.User.Identity.Name}: Finished");
			return GetLink(addSec.sectionid, lastId);

		}

		public static bool DeleteLink(int linkid) {

			Logger.Log($"start for  linkid {linkid} User {HttpContext.Current.User.Identity.Name}");

			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("delete from linksdetail where linkid=@linkid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@linkid", linkid);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException($"DeleteLink({linkid} User={HttpContext.Current.User.Identity.Name} )", ex);
				return false;

			} finally {
				conn.Close();
			}

			Logger.Log($"DeleteLink({linkid} User={HttpContext.Current.User.Identity.Name} ) : Finished");
			return true;

		}

		public static Object Resequence(UserLinksRequenceParams reqParams) {

			if (reqParams.seq_sectionId == -1) {
				reqParams.procName = "ResequenceLinksSections";
			} else {
				reqParams.procName = "ResequenceLinksDetail";
			}

			Logger.Log("Starting");
			Object ret = null;

			try {
			
				//This uses reflection to call whichever method is defined in req.procName.
				//req.procName is also the name of the SP called within each method.

				Type calledType = Type.GetType("HNetPortal.UserLinks");

				ret = (Object) calledType.InvokeMember(
								reqParams.procName,
								BindingFlags.InvokeMethod | BindingFlags.NonPublic |	BindingFlags.Static,
								null, null,
								new Object[] { reqParams });

			} catch (Exception ex) {
				Logger.Log("This wont end well "+ex.Message);
				throw;
			}

			Logger.Log("Worked");
			return ret;

		}

		private static List<UserLinkSecItem> ResequenceLinksSections(UserLinksRequenceParams reqParams) {
			//sectionId is ignored.

			Logger.Log("Start");		
			MySqlConnection conn = new MySqlConnection();
			List<UserLinkSecItem> list = new List<UserLinkSecItem>();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand(reqParams.procName, conn);
				cmd.Prepare();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@xUserName", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@xStartingIdx", reqParams.seq_initialNum);
				cmd.Parameters.AddWithValue("@xIncrementBy", reqParams.seq_incrementBy); ;
				MySqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read()) {
					UserLinkSecItem litem = new UserLinkSecItem {
						sectionid = (int)reader[0],
						orderby = (int)reader[1],
						sectionText = (string)reader[2],
						enabled = (string)reader[3],
						timestamp = (string)reader[4]
					};
					list.Add(litem);
				}

			} catch (Exception ex) {
				Logger.LogException("Error with resequence Section master", ex);
				throw;

			} finally {
				conn.Close();
			}
			return list;

		}

		private static List<UserLinkItem> ResequenceLinksDetail(UserLinksRequenceParams reqParams) {

			Logger.Log("Start");
			
			List<UserLinkItem> list = new List<UserLinkItem>();
			MySqlConnection conn = new MySqlConnection();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand(reqParams.procName, conn);
				cmd.Prepare();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@xUserName", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@xSectionId", reqParams.seq_sectionId);
				cmd.Parameters.AddWithValue("@xStartingIdx", reqParams.seq_initialNum);
				cmd.Parameters.AddWithValue("@xIncrementBy", reqParams.seq_incrementBy);
				MySqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read()) {
					UserLinkItem litem = new UserLinkItem {
						linkid = (int)reader[0],
						sectionid = (int)reader[1],
						orderby = (int)reader[2],
						linkText = (string)reader[3],
						linkURL = (string)reader[4],
						subSectionText = (string)reader[5],
						hoverText = (string)reader[6],
						enabled = (string)reader[7],
						timestamp = (string)reader[8],
						newwindow = (string)reader[9],
						ismenuitem = (string)reader[10]
					};
					list.Add(litem);
				}

			} catch (Exception ex) {
				Logger.LogException("Error with resequence Section Details", ex);
				throw;

			} finally {
				conn.Close();
			}
			return list;

		}
	}
}