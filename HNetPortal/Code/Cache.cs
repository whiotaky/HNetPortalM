using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSHLib;

namespace HNetPortal {
	public static class Cache {
		/// <summary>
		/// Get a cached string from db if its not older than maxageminutes
		/// </summary>
		public static string Get(string cachename, int maxAgeInMinutes) {

			Logger.Log("start");
			string ret = "CACHE NOT FOUND";
			MySqlConnection conn = new MySqlConnection();
			try {
				
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("select data from cache where username=@username and  cachename=@cachename and updated_ts >=  DATE_SUB(NOW(), INTERVAL " + maxAgeInMinutes.ToString() + " MINUTE)", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@cachename", cachename);
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				if (reader.Read()) {
					ret = (string)reader[0];
				}

			} catch (MySqlException ex) {
				//MessageBox.Show(ex.Message);
				ret = "CACHE GET ERROR" + ex.Message;
				Logger.LogException("getCache()", ex);

			} finally {
				conn.Close();
			}

			Logger.Log("end");
			return ret;

		}

		/// <summary>
		/// Create or update a cached data string
		/// </summary>
		public static string Put(string cachename, string data) {

			Logger.Log("start");
			string ret = "";
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("insert into cache values(@username, @cachename, @data, Now()) ON DUPLICATE KEY UPDATE data=@data, updated_ts=Now()", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@cachename", cachename);
				cmd.Parameters.AddWithValue("@data", data);
				cmd.ExecuteNonQuery();

			} catch (MySqlException ex) {
				ret = "CACHE PUT ERROR" + ex.Message;
				Logger.LogException("putCache()", ex);

			} finally {
				conn.Close();
			}

			Logger.Log("end");
			return ret;

		}

	}
}