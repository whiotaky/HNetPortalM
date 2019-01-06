using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WSHLib;

namespace HNetPortal {
	public class CombinedCdAppItem {
		public int id { get; set; }
		public string cdName { get; set; }
		public string volumeName { get; set; }
		public string appName { get; set; }
		public string appType { get; set; }
		public DateTime appDate { get; set; }
		public string filetype { get; set; }
		public string queryStr { get; set; }
		public string Description { get; set; }
		public string longDescription { get; set; }
		public string IDAndType { get; set; }
		public string notes { get; set; }

		public int numFiles { get; set; }
		public long numBytes { get; set; }
		public int numSubDirs { get; set; }
		public string path { get; set; }
		public string fileTypes { get; set; }
		public bool @private { get; set; }

	}

	public class cdCompNameItem {
		public string cdName;
	}

	public enum swdbActionType {
		Create = 1,
		Update,
		Delete
	}

	public class cdCompItem {
		public int id { get; set; }
		public string volumeName { get; set; }
		public string appName { get; set; }
		public string cdName { get; set; }
		public string appType { get; set; }
		public DateTime appDate { get; set; }
		public int numFiles { get; set; }
		public long numBytes { get; set; }
		public int numSubDirs { get; set; }
		public string path { get; set; }
		public string fileTypes { get; set; }
		public string filetype { get; set; }
		public bool @private { get; set; }
		public string notes { get; set; }
		public string queryStr { get; set; }
		public string Description { get; set; }
		public string longDescription { get; set; }
		public string IDAndType { get; set; }
	}

	public class cdIsoItem {
		public int id { get; set; }
		public string appName { get; set; }
		public string cdName { get; set; }
		public string volumeName { get; set; }
		public string appType { get; set; }
		public string notes { get; set; }
		public string filetype { get; set; }

	}

	public class appTypeItem {
		public string type_id { get; set; }
		public string description { get; set; }
		public string longDescription { get; set; }
	}

	public class SoftwareDB {

		public static List<CombinedCdAppItem> getCombinedCdAppList(string appTypeFilter = null) {

			Logger.Log("Start");
			List<CombinedCdAppItem> theList = new List<CombinedCdAppItem>();

			string procName = "GetCombinedCdFiles";
			if (appTypeFilter != null && appTypeFilter != "") {
				procName = "GetCombinedCdFilesByType";
				Logger.Log("Applying appTypeFilter: " + appTypeFilter + " for proc: " + procName);
			}

			string ConnectString = Global.getConnString(SupportedDBTypes.MSSql, "Software");
			SqlConnection connection = new SqlConnection(ConnectString);
			try {
				connection.Open();
				SqlCommand command = new SqlCommand(procName, connection);
				command.CommandType = System.Data.CommandType.StoredProcedure;
				if (appTypeFilter != null && appTypeFilter != "") {
					command.Parameters.AddWithValue("@Type", appTypeFilter);
				}

				using (SqlDataReader rdr = command.ExecuteReader()) {
					while (rdr.Read()) {
						//Console.WriteLine("Product: {0,-35} Total: {1,2}", rdr["ProductName"], rdr["Total"]);
						CombinedCdAppItem buf = new CombinedCdAppItem();
						buf.id = (int)rdr["id"];
						buf.cdName = (string)rdr["cdName"];
						buf.volumeName = (string)rdr["volumeName"];
						buf.appName = (string)rdr["appName"];
						buf.appType = (string)rdr["appType"];
						buf.appDate = (DateTime)rdr["appDate"];
						buf.filetype = (string)rdr["filetype"];
						buf.queryStr = (string)rdr["queryStr"];
						buf.Description = (string)rdr["Description"];
						buf.longDescription = (string)rdr["longDescription"];
						buf.IDAndType = (string)rdr["ID And Type"];
						buf.notes = (string)rdr["notes"];
						buf.numFiles = (int)rdr["numFiles"];
						buf.numBytes = (long)rdr["numBytes"];
						buf.numSubDirs = (int)rdr["numSubDirs"];
						buf.path = (rdr["path"] == DBNull.Value) ? "" : (string)rdr["path"];
						buf.fileTypes = (string)rdr["fileTypes"];
						buf.@private = ((int)rdr["private"] == 1);  //WTF?

						theList.Add(buf);
					}
				}
			} catch (Exception ex) {
				Logger.LogException("Exception", ex);
				throw;
			} finally {
				connection.Close();
				Logger.Log("succeeded");
			}

			return theList;
		}

		public static List<cdCompNameItem> getcdCompNameList() {

			Logger.Log("Start");
			List<cdCompNameItem> theList = new List<cdCompNameItem>();

			string ConnectString = Global.getConnString(SupportedDBTypes.MSSql, "Software");
			SqlConnection connection = new SqlConnection(ConnectString);
			try {
				connection.Open();
				SqlCommand command = new SqlCommand("SELECT distinct(cdName) from cdComp order by cdName", connection) {
					CommandType = System.Data.CommandType.Text
				};

				using (SqlDataReader rdr = command.ExecuteReader()) {
					while (rdr.Read()) {
						//Console.WriteLine("Product: {0,-35} Total: {1,2}", rdr["ProductName"], rdr["Total"]);
						cdCompNameItem buf = new cdCompNameItem {
							cdName = (string)rdr["cdName"]
						};
						theList.Add(buf);
					}
				}
			} catch (Exception ex) {
				Logger.LogException("Exception", ex);
			} finally {
				connection.Close();
				Logger.Log("succeeded");
			}

			return theList;
		}

		public static List<cdCompItem> getCdCompAppList(string cdName, string appTypeFilter = null) {

			Logger.Log("Start");
			List<cdCompItem> theList = new List<cdCompItem>();

			string atFilter = "%";
			if (appTypeFilter != null && appTypeFilter != "") {
				atFilter = string.Format("%{0}%", appTypeFilter);
				Logger.Log("Applying appTypeFilter: " + atFilter);
			}

			string ConnectString = Global.getConnString(SupportedDBTypes.MSSql, "Software");
			SqlConnection connection = new SqlConnection(ConnectString);
			try {
				connection.Open();
				SqlCommand command = new SqlCommand("Select id, volumeName,appName,cdName,appType,appDate,numFiles,numBytes,numSubDirs,path, fileTypes,private as xprivate,notes from cdComp  where cdName=@cdName and appType like @appTypeFilter order by cdName", connection) {
					CommandType = System.Data.CommandType.Text
				};
				command.Prepare();
				command.Parameters.AddWithValue("@cdName", cdName);
				command.Parameters.AddWithValue("@appTypeFilter", atFilter);

				using (SqlDataReader rdr = command.ExecuteReader()) {
					while (rdr.Read()) {
						//Console.WriteLine("Product: {0,-35} Total: {1,2}", rdr["ProductName"], rdr["Total"]);
						cdCompItem buf = new cdCompItem {
							id = (int)rdr["id"],
							volumeName = (string)rdr["volumeName"],
							appName = (string)rdr["appName"],
							cdName = (string)rdr["cdName"],
							appType = (string)rdr["appType"],
							appDate = (DateTime)rdr["appDate"],
							numFiles = (int)rdr["numFiles"],
							numBytes = (long)rdr["numBytes"],
							numSubDirs = (int)rdr["numSubDirs"],
							path = (rdr["path"] == DBNull.Value) ? "" : (string)rdr["path"],
							fileTypes = (string)rdr["fileTypes"],
							filetype = "cdComp",
							@private = (bool)rdr["xprivate"],
							notes = (string)rdr["notes"]
						};
						theList.Add(buf);
						Logger.Log(string.Format("Added {0} to the list", buf.appName));
					}
				}
			} catch (Exception ex) {
				Logger.LogException("Exception", ex);
			} finally {
				connection.Close();
				Logger.Log("succeeded");
			}

			Logger.Log(string.Format("returning list of size{0}", theList.Count()));
			return theList;
		}


		public static List<cdIsoItem> getIsoCdList(string appTypeFilter = null) {

			Logger.Log("Start");
			List<cdIsoItem> theList = new List<cdIsoItem>();

			string atFilter = "%";
			if (appTypeFilter != null && appTypeFilter != "") {
				atFilter = string.Format("%{0}%", appTypeFilter);
				Logger.Log("Applying appTypeFilter: " + atFilter);
			}

			string ConnectString = Global.getConnString(SupportedDBTypes.MSSql, "Software");
			SqlConnection connection = new SqlConnection(ConnectString);
			try {
				connection.Open();
				SqlCommand command = new SqlCommand("Select * from cdIso where appType like @appTypeFilter order by appName asc, cdName asc", connection) {
					CommandType = System.Data.CommandType.Text
				};
				command.Parameters.AddWithValue("@appTypeFilter", atFilter);

				using (SqlDataReader rdr = command.ExecuteReader()) {
					while (rdr.Read()) {
						//Console.WriteLine("Product: {0,-35} Total: {1,2}", rdr["ProductName"], rdr["Total"]);
						cdIsoItem buf = new cdIsoItem {
							id = (int)rdr["id"],
							cdName = (string)rdr["cdName"],
							volumeName = (string)rdr["volumeName"],
							appName = (string)rdr["appName"],
							appType = (string)rdr["appType"],
							notes = (string)rdr["notes"],
							filetype = "cdIso"
						};
						theList.Add(buf);
					}
				}
			} catch (Exception ex) {
				Logger.LogException("Exception", ex);
			} finally {
				connection.Close();
				Logger.Log("succeeded");
			}

			return theList;
		}

		public static List<appTypeItem> getAppTypeList() {

			Logger.Log("Start");
			List<appTypeItem> theList = new List<appTypeItem>();

			string ConnectString = Global.getConnString(SupportedDBTypes.MSSql, "Software");
			SqlConnection connection = new SqlConnection(ConnectString);
			try {
				connection.Open();
				SqlCommand command = new SqlCommand("Select * from appTypes order by description", connection) {
					CommandType = System.Data.CommandType.Text
				};

				using (SqlDataReader rdr = command.ExecuteReader()) {
					while (rdr.Read()) {
						//Console.WriteLine("Product: {0,-35} Total: {1,2}", rdr["ProductName"], rdr["Total"]);
						appTypeItem buf = new appTypeItem {
							type_id = (string)rdr["type_id"],
							description = (string)rdr["description"],
							longDescription = (string)rdr["longDescription"]
						};
						theList.Add(buf);
					}
				}
			} catch (Exception ex) {
				Logger.LogException("Exception", ex);
			} finally {
				connection.Close();
				Logger.Log("succeeded");
			}

			return theList;
		}


		public static List<CombinedCdAppItem> getSearchResults(string searchTerm) {

			Logger.Log("Start");
			List<CombinedCdAppItem> theList = new List<CombinedCdAppItem>();

			Logger.Log(string.Format("Search Term='{0}'", searchTerm));

			string ConnectString = Global.getConnString(SupportedDBTypes.MSSql, "Software");
			SqlConnection connection = new SqlConnection(ConnectString);
			try {
				connection.Open();
				SqlCommand command = new SqlCommand("SearchAll_Deep", connection) {
					CommandType = System.Data.CommandType.StoredProcedure
				};
				command.Parameters.AddWithValue("@searchString", string.Format("%{0}%", searchTerm));


				using (SqlDataReader rdr = command.ExecuteReader()) {
					while (rdr.Read()) {
						//Console.WriteLine("Product: {0,-35} Total: {1,2}", rdr["ProductName"], rdr["Total"]);
						CombinedCdAppItem buf = new CombinedCdAppItem {
							id = (int)rdr["id"],
							cdName = (string)rdr["cdName"],
							volumeName = (string)rdr["volumeName"],
							appName = (string)rdr["appName"],
							appType = (string)rdr["appType"],
							appDate = (DateTime)rdr["appDate"],
							filetype = (string)rdr["filetype"],
							queryStr = (string)rdr["queryStr"],
							Description = (string)rdr["Description"],
							longDescription = (string)rdr["longDescription"],
							IDAndType = (string)rdr["ID And Type"],
							notes = (string)rdr["notes"],
							numFiles = (int)rdr["numFiles"],
							numBytes = (long)rdr["numBytes"],
							numSubDirs = (int)rdr["numSubDirs"],
							path = (rdr["path"] == DBNull.Value) ? "" : (string)rdr["path"],
							fileTypes = (string)rdr["fileTypes"],
							@private = ((int)rdr["private"] == 1)  //WTF?						
						};

						theList.Add(buf);
					}
				}
			} catch (Exception ex) {
				Logger.LogException("Exception", ex);
			} finally {
				connection.Close();
				Logger.Log("succeeded");
			}

			return theList;
		}

		public static void writeAppType(string type_id_orig, appTypeItem rec, swdbActionType action) {

			string sql = "";

			string ConnectString = Global.getConnString(SupportedDBTypes.MSSql, "Software");
			SqlConnection connection = new SqlConnection(ConnectString);
			SqlCommand command = new SqlCommand {
				CommandType = System.Data.CommandType.Text
			};

			if (action == swdbActionType.Update) {
				Logger.Log("Update " + type_id_orig);
				sql = "update AppTypes set type_id=@type_id, description=@descr, longDescription=@ldescr where type_id=@type_id_orig";
				command.CommandText = sql;
				command.Parameters.AddWithValue("@type_id", rec.type_id.ToUpper());
				command.Parameters.AddWithValue("@descr", rec.description);
				command.Parameters.AddWithValue("@ldescr", rec.longDescription);
				command.Parameters.AddWithValue("@type_id_orig", type_id_orig.ToUpper());

			} else if (action == swdbActionType.Create) {
				Logger.Log("Update " + rec.type_id);
				sql = "insert into AppTypes values(@type_id,@descr,@ldescr)";
				command.CommandText = sql;
				command.Parameters.AddWithValue("@type_id", rec.type_id.ToUpper());
				command.Parameters.AddWithValue("@descr", rec.description);
				command.Parameters.AddWithValue("@ldescr", rec.longDescription);

			} else if (action == swdbActionType.Delete) {
				Logger.Log("Delete " + type_id_orig);
				sql = "delete from AppTypes where type_id=@type_id_orig";
				command.CommandText = sql;
				command.Parameters.AddWithValue("@type_id_orig", type_id_orig.ToUpper());

			} else {
				Logger.Log("Throwing an exception--Unknown handled dbActionType");
				throw new Exception("Unknown handled dbActionType");
			}

			try {
				connection.Open();
				command.Connection = connection;
				command.ExecuteNonQuery();

			} catch (SqlException ex) {
				Logger.LogException("SQLException #" + ex.Number.ToString(), ex);
				throw;
			} catch (Exception ex) {
				Logger.LogException("Exception", ex);
				throw;
			} finally {
				connection.Close();
			}

			Logger.Log("Finished");
		}

		public static string writeIsoItem(cdIsoItem item, swdbActionType action) {

			string sql = "";
			string ret = "";

			string ConnectString = Global.getConnString(SupportedDBTypes.MSSql, "Software");
			SqlConnection connection = new SqlConnection(ConnectString);
			SqlCommand command = new SqlCommand {
				CommandType = System.Data.CommandType.Text
			};

			if (action == swdbActionType.Update) {
				Logger.Log("Update " + item.id);
				sql = "update cdIso set appName=@appName, volumeName=@volumeName, cdName=@cdName, appType=@appType, notes=@notes where id=@id";
				command.CommandText = sql;
				command.Parameters.AddWithValue("@id", item.id);
				command.Parameters.AddWithValue("@appName", item.appName);
				command.Parameters.AddWithValue("@volumeName", item.volumeName);
				command.Parameters.AddWithValue("@cdName", item.cdName);
				command.Parameters.AddWithValue("@appType", item.appType);
				command.Parameters.AddWithValue("@notes", item.notes);

			} else if (action == swdbActionType.Create) {
				Logger.Log("Create " + item.id);
				sql = "insert into cdIso values(@appName,@volumeName,@cdName,@appType,@notes)";
				command.CommandText = sql;
				//command.Parameters.AddWithValue("@id", item.id);
				command.Parameters.AddWithValue("@appName", item.appName);
				command.Parameters.AddWithValue("@volumeName", item.volumeName);
				command.Parameters.AddWithValue("@cdName", item.cdName);
				command.Parameters.AddWithValue("@appType", item.appType);
				command.Parameters.AddWithValue("@notes", item.notes);

			} else if (action == swdbActionType.Delete) {
				Logger.Log("Delete " + item.id);
				sql = "delete from cdIso where id=@id";
				command.CommandText = sql;
				command.Parameters.AddWithValue("@id", item.id);
			} else {
				Logger.Log("Throwing an exception--Unknown handled dbActionType");
				throw new Exception("Unknown handled dbActionType");
			}

			try {
				connection.Open();
				command.Connection = connection;
				command.ExecuteNonQuery();

				if (action == swdbActionType.Create) {
					SqlCommand command2 = new SqlCommand {
						CommandType = System.Data.CommandType.Text,
						Connection = connection,
						CommandText = "select max(id) as lastId from cdIso"
					};
					//command2.CommandText = "select SCOPE_IDENTITY() as lastId";
					using (SqlDataReader rdr = command2.ExecuteReader()) {
						rdr.Read();
						int lastId = (int)rdr["lastId"];
						Logger.Log("Id of new ISO record=" + lastId.ToString());
						ret = "AddRec=" + lastId.ToString();
					}
				} else if (action == swdbActionType.Delete) {
					Logger.Log("Id of deleted ISO record=" + item.id);
					ret = "DelRec=" + item.id;
				}


			} catch (SqlException ex) {
				Logger.LogException("SQLException #" + ex.Number.ToString(), ex);
				throw;
			} catch (Exception ex) {
				Logger.LogException("Exception", ex);
				throw;
			} finally {
				connection.Close();
			}

			Logger.Log("Finished");
			return ret;
		}

		public static string writecdCompItem(cdCompItem item, swdbActionType action) {

			string sql = "";
			string ret = "";

			string ConnectString = Global.getConnString(SupportedDBTypes.MSSql, "Software");
			SqlConnection connection = new SqlConnection(ConnectString);
			SqlCommand command = new SqlCommand {
				CommandType = System.Data.CommandType.Text
			};

			if (action == swdbActionType.Update) {
				Logger.Log("Update " + item.id);
				sql = "update cdComp set appName=@appName, volumeName=@volumeName, cdName=@cdName, appType=@appType, notes=@notes, appDate=@appDate, fileTypes=@fileTypes, numFiles=@numFiles, numBytes=@numBytes, numSubDirs=@numSubDirs, path=@path, private=@private where id=@id";
				command.CommandText = sql;
				command.Parameters.AddWithValue("@id", item.id);
				command.Parameters.AddWithValue("@appName", item.appName);
				command.Parameters.AddWithValue("@volumeName", item.volumeName);
				command.Parameters.AddWithValue("@cdName", item.cdName);
				command.Parameters.AddWithValue("@appType", item.appType);
				command.Parameters.AddWithValue("@notes", item.notes);
				command.Parameters.AddWithValue("@appDate", item.appDate);
				command.Parameters.AddWithValue("@fileTypes", item.fileTypes);
				command.Parameters.AddWithValue("@numFiles", item.numFiles);
				command.Parameters.AddWithValue("@numBytes", item.numBytes);
				command.Parameters.AddWithValue("@numSubDirs", item.numSubDirs);
				command.Parameters.AddWithValue("@path", item.path);
				command.Parameters.AddWithValue("@private", item.@private);

			} else if (action == swdbActionType.Create) {
				Logger.Log("Create " + item.id);
				sql = "insert into cdComp (appName,volumeName,cdName,appType,notes,appDate, fileTypes, numFiles, numBytes, numSubDirs, path, private) values(@appName,@volumeName,@cdName,@appType,@notes,@appDate, @fileTypes, @numFiles, @numBytes, @numSubDirs, @path, @private)";
				command.CommandText = sql;
				//command.Parameters.AddWithValue("@id", item.id);
				command.Parameters.AddWithValue("@appName", item.appName);
				command.Parameters.AddWithValue("@volumeName", item.volumeName);
				command.Parameters.AddWithValue("@cdName", item.cdName);
				command.Parameters.AddWithValue("@appType", item.appType);
				command.Parameters.AddWithValue("@notes", item.notes);
				command.Parameters.AddWithValue("@appDate", item.appDate);
				command.Parameters.AddWithValue("@fileTypes", item.fileTypes);
				command.Parameters.AddWithValue("@numFiles", item.numFiles);
				command.Parameters.AddWithValue("@numBytes", item.numBytes);
				command.Parameters.AddWithValue("@numSubDirs", item.numSubDirs);
				command.Parameters.AddWithValue("@path", item.path);
				command.Parameters.AddWithValue("@private", item.@private);

			} else if (action == swdbActionType.Delete) {
				Logger.Log("Delete " + item.id);
				sql = "delete from cdComp where id=@id";
				command.CommandText = sql;
				command.Parameters.AddWithValue("@id", item.id);
			} else {
				Logger.Log("Throwing an exception--Unknown handled dbActionType");
				throw new Exception("Unknown handled dbActionType");
			}

			try {
				connection.Open();
				command.Connection = connection;
				command.ExecuteNonQuery();

				if (action == swdbActionType.Create) {
					SqlCommand command2 = new SqlCommand {
						CommandType = System.Data.CommandType.Text,
						Connection = connection,
						CommandText = "select max(id) as lastId from cdComp"
					};
					//command2.CommandText = "select SCOPE_IDENTITY() as lastId";
					using (SqlDataReader rdr = command2.ExecuteReader()) {
						rdr.Read();
						int lastId = (int)rdr["lastId"];
						Logger.Log("Id of new CdComp record=" + lastId.ToString());
						ret = "AddRec=" + lastId.ToString();
					}
				} else if (action == swdbActionType.Delete) {
					Logger.Log("Id of deleted cdComp record=" + item.id);
					ret = "DelRec=" + item.id;
				}

			} catch (SqlException ex) {
				Logger.LogException("SQLException #" + ex.Number.ToString(), ex);
				throw;
			} catch (Exception ex) {
				Logger.LogException("Exception", ex);
				throw;
			} finally {
				connection.Close();
			}

			Logger.Log("Finished");
			return ret;

		}
	}

}