using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSHLib;

namespace HNetPortal {

	public class RSSMasterItem {
		public int feedid { get; set; }
		public string feedName { get; set; }
		public byte feedType { get; set; }
		public string feedURL { get; set; }
		public string cacheFilePrefix { get; set; }
		public string enabled { get; set; }
	}

	public static class RSSMaster {

		public static RSSMasterItem GetItem(int feedId) {

			Logger.Log($"Start GetItem({feedId})");

			MySqlConnection conn = new MySqlConnection();
			List<RSSMasterItem> list = new List<RSSMasterItem>();
			RSSMasterItem fmi = new RSSMasterItem();

			try {
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT feedid, feedName,  feedType, feedURL, cacheFilePrefix, enabled FROM feedsmaster where feedid=@feedid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@feedid", feedId);
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				reader.Read();
				fmi = new RSSMasterItem {
					feedid = feedId,
					feedName = (string)reader[1],
					feedType = (byte)reader[2],
					feedURL = (string)reader[3],
					cacheFilePrefix = (string)reader[4],
					enabled = (string)reader[5]
				};

			} catch (Exception ex) {
				Logger.LogException($"GetItem({feedId}", ex);
				throw;
			} finally {
				conn.Close();
			}

			Logger.Log($"GetItem({feedId} Finished");
			return fmi;

		}

		public static RSSMasterItem UpdateItem(RSSMasterItem updItem) {
			Logger.Log($"UpdateItem({updItem.feedid})");
			MySqlConnection conn = new MySqlConnection();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("Update feedsmaster set feedName=@feedName, feedType=@feedType, feedURL=@feedURL, cacheFilePrefix=@cacheFilePrefix, enabled=@enabled where feedid=@feedid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@feedid", updItem.feedid);
				cmd.Parameters.AddWithValue("@feedName", updItem.feedName);
				cmd.Parameters.AddWithValue("@feedType", updItem.feedType);
				cmd.Parameters.AddWithValue("@feedURL", updItem.feedURL);
				cmd.Parameters.AddWithValue("@cacheFilePrefix", updItem.cacheFilePrefix);
				cmd.Parameters.AddWithValue("@enabled", updItem.enabled);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException($"UpdateItem({updItem.feedid}", ex);
				throw;

			} finally {
				conn.Close();
			}

			Logger.Log($"UpdateItem({updItem.feedid}) Finished");
			return GetItem(updItem.feedid);

		}

		public static RSSMasterItem AddItem(RSSMasterItem addItem) {
			Logger.Log($"AddItem");

			MySqlConnection conn = new MySqlConnection();
			RSSMasterItem ret = new RSSMasterItem();
			long nextFeedID = 0;

			try {

				//ignore what is provided in feedid. Since this is an add and it needs to be
				//unique, get next available number from the table since its not an auto-index table.

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("select max(feedid) + 1 as nextId from feedsmaster;", conn);
				cmd.Prepare();
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				reader.Read();
				nextFeedID = (long)reader[0];
				Logger.Log("nextID is " + nextFeedID.ToString());
				reader.Close();

				cmd = new MySqlCommand("insert into feedsmaster values(@feedid,@feedName,@feedType,@feedURL,@cacheFilePrefix, @enabled)", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@feedid", nextFeedID);
				cmd.Parameters.AddWithValue("@feedName", addItem.feedName);
				cmd.Parameters.AddWithValue("@feedType", addItem.feedType);
				cmd.Parameters.AddWithValue("@feedURL", addItem.feedURL);
				cmd.Parameters.AddWithValue("@cacheFilePrefix", addItem.cacheFilePrefix);
				cmd.Parameters.AddWithValue("@enabled", addItem.enabled);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException($"AddItem({addItem.feedid})", ex);
				throw;
			} finally {
				conn.Close();
			}

			Logger.Log($"AddItem({nextFeedID}) Finished. Added okay.");
			return GetItem((int)nextFeedID);

		}

		public static bool DeleteItem(int feedid) {
			Logger.Log($"DeleteItem");
			MySqlConnection conn = new MySqlConnection();
			try {
				Logger.Log($"DeleteItem({feedid}) deleting feedmaster record");
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("delete from feedsmaster where feedid=@feedid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@feedid", feedid);
				cmd.ExecuteNonQuery();

				Logger.Log($"DeleteItem({feedid}), deleting any userfeedprefs for feedid");
				cmd = new MySqlCommand("delete from userfeedprefs where feedid=@feedid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@feedid", feedid);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException($"DeleteItem({feedid}) exception", ex);
				return false;

			} finally {
				conn.Close();
			}

			Logger.Log($"DeleteItem({feedid}) Finished");
			return true;
		}


	}
}