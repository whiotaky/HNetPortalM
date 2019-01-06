using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WSHLib;

namespace HNetPortal {
	public static class ShipperTrack {
		public  class ShipperTrackItem {
			public string trackingNo { get; set; }
			public string shipperCode { get; set; }
			public DateTime update_ts { get; set; }
			public string userName { get; set; }
		}

		public static void ShipperTrackInsert(string trackingNo, string shipperCode) {

			MySqlConnection conn = new MySqlConnection();
		
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("ShipperTrackInsert", conn);
				cmd.Prepare();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@xuserName", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@xtrackingNo", trackingNo);
				cmd.Parameters.AddWithValue("@xshipperCode", shipperCode);
				int x = cmd.ExecuteNonQuery();				

			} catch (Exception ex) {
				Logger.LogException("Error on shipperTrack Insert db call", ex);
				throw;

			} finally {
				conn.Close();
			}

		}

		public static List<ShipperTrackItem> ShipperTrackGetList(string shipperCode) {
			MySqlConnection conn = new MySqlConnection();
			List<ShipperTrackItem> list = new List<ShipperTrackItem>();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("ShipperTrackGetList", conn);
				cmd.Prepare();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@xuserName", HttpContext.Current.User.Identity.Name);
				cmd.Parameters.AddWithValue("@xshipperCode", shipperCode);
				MySqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read()) {

					ShipperTrackItem sitem = new ShipperTrackItem {
						trackingNo = (string)reader[0],
						shipperCode = (string)reader[1],						
						update_ts = (DateTime)reader[2],
						userName = (string)reader[3]
					};
					list.Add(sitem);

				}

			} catch (Exception ex) {
				Logger.LogException("Error with ShipperTrackGetList", ex);
				throw;

			} finally {
				conn.Close();
			}

			return list;
		}

	}

	
}