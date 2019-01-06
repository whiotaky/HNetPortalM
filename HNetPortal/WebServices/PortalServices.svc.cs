using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.Services;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.IO;
using Renci.SshNet;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data.SqlClient;
using KeePassLib;
using Newtonsoft.Json;
using WSHLib;
using WSHLib.Network;

//http://www.ogris.de/sshesxi/   HOW-TO create/install key pairs so you can remote into esxi and troll servers

namespace HNetPortal.WebServices {


	[Obsolete("Deprecated, please use RSSMasterItem instead.")]
	/// <summary>
	/// Deprecated 3/9/2018 (migrated to class)
	/// </summary>
	public class FeedMasterItem {
		public int feedid { get; set; }
		public string feedName { get; set; }
		public byte feedType { get; set; }
		public string feedURL { get; set; }
		public string cacheFilePrefix { get; set; }
		public string enabled { get; set; }
	}

	[Obsolete("Deprecated, please use QuoteItem instead.")]
	/// <summary>
	/// Deprecated 3/4/2018 (migrated to class)
	/// </summary>
	public class quoteItem {
		public string Symbol { get; set; }
		public string Price { get; set; }
		public string Change { get; set; }
		public string ChangeTrend { get; set; }
		public string Name { get; set; }
	}

	[Obsolete("Deprecated, please use UserLinkSection instead.")]
	/// <summary>
	/// Deprecated 3/10/2018 (migrated to class)
	/// </summary>
	public class userLinkSecItem {
		public int sectionid { get; set; }
		public int orderby { get; set; }
		public string sectionText { get; set; }
		public string enabled { get; set; }
		public string timestamp { get; set; }
	}

	[Obsolete("Deprecated, please use UserLinkSection instead.")]
	/// <summary>
	/// Deprecated 3/10/2018 (migrated to class)
	/// </summary>
	public class userLinkItem {
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


	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class PortalServices : WebService {

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		///  Deprecated 3/3/2018 
		/// </summary>		
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]				
		public string getNodesStatuses(bool allowCached) {

			//DEBUG Code
			//return "[{\"nodeName\": \"plex\", \"nodeStatus\": \"up\"},{\"nodeName\": \"deb\", \"nodeStatus\": \"down\"}]";
			//return "[{\"nodeName\": \"plex\", \"nodeStatus\": \"up\"}]";

			Logger.Log("start for user " + User.Identity.Name);
			if (allowCached) {
				string fromCache = getCache("netnodes", 30);
				if (!fromCache.Contains("CACHE GET ERROR") &&
				!fromCache.Contains("CACHE NOT FOUND")) {
					Logger.Log("got from cache so ending");
					return fromCache;
				}
			} else {
				Logger.Log("allowCached FALSE, so getting fresh for" + User.Identity.Name);
			}

			NetNodes n = new NetNodes();
			string ret = string.Empty; //Due to deprecation! .getList();
			putCache("netnodes", ret);
			Logger.Log("End");
			return ret;	
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/4/2018 
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getQuotes(string symbols, bool allowCached) {
			/*
			<?xml version="1.0" encoding="iso-8859-1" ?> 
			 <FQLR>
				<STATUS ERROR_CODE="0" ERROR_TEXT="" /> 
				<Q_L T="I" S=".DJI" X="03/03/2016|4:20:01pm|16,943.90|44.58|DJ INDUSTRIAL AVERAGE|U|16,944.31|16,820.73|0.26|0.00|0.00|16,943.90|16,896.17|03/03/2016|18,351.36|15,370.33|05/19/2015|08/24/2015|124990177|FPS.REC.^DJI.NaE|91,113,445|03/03/2016 16:20:01" /> 
				<Q_L T="I" S=".IXIC" X="03/03/2016|5:15:00pm|4,707.423|4.001|NASDAQ COMPOSITE INDEX COMP|D|4,707.719|4,674.46|0.09|8,183.837|0.00|4,703.42|4,698.383|03/02/2016|5,231.942|4,209.759|07/20/2015|02/11/2016|12499D906|FPS.REC.^IXIC.NaE|444,975,299|03/03/2016 17:15:00" /> 
				<Q_L T="E" S="PM-G-1OZ" X="03/04/2016|09:12:06|1263.14|2.9|Gold|0.00|0|0|0|0.00|4|U||N/A|N/A|N/A|N/A|N/A|N/A|N/A|0.00|00/00/0000||||USD|0.00|00000|0.00|0.00|0.00|0.00|COMX|000|0.00|00/00/0000|00/0902000|||0|0||0|0|PM-G-1OZ|Metal|N|" /> 

			</FQLR>
			*/
			Logger.Log("start for user " + User.Identity.Name);
			if (allowCached) {
				string fromCache = getCache("quotes", 30);
				if (!fromCache.Contains("CACHE GET ERROR") &&
				!fromCache.Contains("CACHE NOT FOUND")) {
					Logger.Log("got from cache, so ending");
					return fromCache;
				}
			}

			Logger.Log("fetching new");
			List<quoteItem> list = new List<quoteItem>();
			try {
				string docStr = "http://quoteshost/cgi-bin/quote.pl?NAME=QUOTELITE&XML=n&p1=" + symbols + "&p2=y";
				XDocument doc = XDocument.Load(docStr);
				var allElements = doc.Descendants();
				var quotes = doc.Descendants().Where(x => x.Attribute("S") != null);

				foreach (var quote in quotes) {
					quoteItem qitem = new quoteItem();

					string symbol = quote.Attribute("S").Value;
					string ttype = quote.Attribute("T").Value;
					string payload = quote.Attribute("X").Value;
					var arr = payload.Split('|');

					qitem.Symbol = symbol;
					switch (ttype) {
						case "I": //Index
							qitem.Name = arr[4];
							qitem.Price = arr[2];
							qitem.Change = arr[3];
							break;
						case "E": //Equity
							qitem.Name = arr[4];
							qitem.Price = arr[2];
							qitem.Change = arr[3];
							break;
						case "S": //Select MF
							qitem.Name = arr[7];
							qitem.Price = arr[3];
							qitem.Change = arr[2];
							break;
						case "M": //MF
							qitem.Name = arr[6];
							qitem.Price = arr[2];
							qitem.Change = arr[1];
							break;
						case "R": //MF also
							qitem.Name = arr[6];
							qitem.Price = arr[2];
							qitem.Change = arr[1];
							break;
					}

					if (qitem.Change.Contains("-")) {
						qitem.ChangeTrend = "Down";
					} else {
						qitem.ChangeTrend = "Up";
					}

					decimal d = Convert.ToDecimal(qitem.Price);
					qitem.Price = String.Format("{0:n}", d);

					d = Convert.ToDecimal(qitem.Change);
					qitem.Change = d.ToString("0.00");
					if (!ttype.Equals("I")) {
						qitem.Price = "$" + qitem.Price;
					}

					list.Add(qitem);

				}
			} catch (Exception ex) {
				Logger.LogException("getQuotes", ex);
				quoteItem qitem = new quoteItem();
				qitem.Symbol = "Error";
				qitem.Name = "ERROR";
				qitem.Price = "0.00";
				qitem.Change = "Down";
				list.Add(qitem);
			}

			//return "[" +	"{\"Symbol\": \"ERR\", \"Price\": \"ERR\",\"Change\": \"ERR\"}" +"]";
			var jsonSerialiser = new JavaScriptSerializer();
			putCache("quotes", jsonSerialiser.Serialize(list));
			Logger.Log("end");
			return jsonSerialiser.Serialize(list);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/6/2018 
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
		public string getCalEvents(string start, string end) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log(string.Format("from={0} to={1}", start, end));

			List<ICal.EventItem> list = new List<ICal.EventItem>();
			var jsonSerialiser = new JavaScriptSerializer();
			MySqlConnection conn = new MySqlConnection();
			try {
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("select  date_format(calDate, '%Y-%m-%d')  as eventDate, content from calendar where username=@username and (calDate >=@fromDate  and calDate <=@toDate)", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.Parameters.AddWithValue("@fromDate", start);
				cmd.Parameters.AddWithValue("@toDate", end);

				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				while (reader.Read()) {
					ICal.EventItem eitem = new ICal.EventItem {
						start = (string)reader[0],
						title = (string)reader[1],
						allDay = "true"
					};
					list.Add(eitem);
				}

			} catch (MySqlException ex) {
				Logger.LogException("getCalEvents() Exception: ", ex);

			} finally {
				conn.Close();
			}

			Logger.Log("Ending with return list");
			return jsonSerialiser.Serialize(list);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/6/2018 
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string editCalEvent(string calDate, string calContent) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			Logger.Log(string.Format("calDate={0} calContent(length)={1}", calDate, calContent.Length));

			Logger.Log("start for user " + User.Identity.Name);
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("replace into calendar set username=@username, caldate=@caldate, content=@content", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.Parameters.AddWithValue("@caldate", calDate);
				cmd.Parameters.AddWithValue("@content", calContent);
				cmd.ExecuteNonQuery();

			} catch (MySqlException ex) {
				Logger.LogException("getCalEvent() Exception: ", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";
			} finally {
				conn.Close();
			}
			Logger.Log("End");
			return "[{\"status\": \"Success\"}]";

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/6/2018 
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string deleteCalEvent(string calDate, string calContent) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			Logger.Log(string.Format("calDate={0} (Ignoring content)", calDate));

			Logger.Log("start for user " + User.Identity.Name);
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("delete from calendar where username=@username and caldate=@caldate", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.Parameters.AddWithValue("@caldate", calDate);

				cmd.ExecuteNonQuery();

			} catch (MySqlException ex) {
				Logger.LogException("deleteCalEvent() Exception: ", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";
			} finally {
				conn.Close();
			}
			Logger.Log("End");
			return "[{\"status\": \"Success\"}]";

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/3/2018 
		/// </summary> 
		private string getCache(string cachename, int maxAgeInMinutes) {

			Logger.Log("start");
			string ret = "CACHE NOT FOUND";
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("select data from cache where username=@username and  cachename=@cachename and updated_ts >=  DATE_SUB(NOW(), INTERVAL " + maxAgeInMinutes.ToString() + " MINUTE)", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
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

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/4/2018 
		/// </summary> 
		private string putCache(string cachename, string data) {

			Logger.Log("start");
			string ret = "";
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("insert into cache values(@username, @cachename, @data, Now()) ON DUPLICATE KEY UPDATE data=@data, updated_ts=Now()", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
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

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/5/2018 
		/// </summary> 
		//https://blogs.msdn.microsoft.com/carlosfigueira/2008/04/17/wcf-raw-programming-model-web/
		[OperationContract, WebGet]
		public System.IO.Stream getLog(string whichLog, int numLines) {

			//http://localhost:63976/WebServices/PortalServicesText.svc/getLog?whichLog=foobar

			StringBuilder result = new StringBuilder();

			Logger.Log("which=" + whichLog);

			if (whichLog.Equals("ESXI")) {
				// see http://www.ogris.de/sshesxi/
				//update: 3/10/2017:  logreaderpx account works as I created /etc/ssh/keys-logreader, then 
				//authorized_keys with contents of logreader's public key, and chown that file to logreaderpx. 
				//It was persisted by leveraging the local.sh script on the esxi server to copy the file into
				//place and set its perms/owner when the esxi server reboots, similar to how ghetto backup does.
				try {
					//fetch log file from esxi server via sftp
					string localFileName = (string)ConfigurationManager.AppSettings["WorkDir"] + "/esxiBackup.log";
					string privateKeyFileName = Server.MapPath(@"~/App_Data") + "/lrpx_esxi";
					string remoteFileName = (string)ConfigurationManager.AppSettings["LOG_" + whichLog];
					PrivateKeyFile pkf = new PrivateKeyFile(privateKeyFileName);

					using (var sftp = new SftpClient("esxi", "logreaderpx", pkf)) {
						sftp.Connect();
						Logger.Log("esxi log connection good");
						using (var file = File.OpenWrite(localFileName)) {
							sftp.DownloadFile(remoteFileName, file);
						}
						sftp.Disconnect();
						Logger.Log("got esxi connection and disconnected");
					}

					//Read in file, and filter it down to lines that contain "info"
					List<string> rawFile = File.ReadLines(localFileName).ToList();
					List<string> filtered = new List<string>();
					for (int i = 0; i < rawFile.Count; i++) {
						if (rawFile[i].Contains("info")) {
							filtered.Add(rawFile[i] + "\r\n");
						}
					}

					//position file "pointer" to the end, minus number of lines requested
					int startAt = filtered.Count - numLines;
					if (startAt < 0) {
						startAt = 0;
					}

					//output lines
					var last = filtered.Skip(startAt);
					foreach (var line in last) {
						result.Append((string)line);
					}

				} catch (Exception ex) {
					Logger.LogException("getLog ESXi EXCEPTION", ex);
					result.Clear();
					result.Append(ex.Message);
				}

			} else {
				string unc = (string)ConfigurationManager.AppSettings["LOG_" + whichLog];

				if (whichLog.Equals("TFSBACKUP")) {
					//TFS has a set of unique backup filenames.  unc should be 
					//set to the TFS backup directory name, so we want to set it 
					//to the most recent FULL backup log filename.
					try {
						string latestFile = Directory.GetFiles(unc, "Full_*.log").OrderByDescending(d => new FileInfo(d).CreationTime).First();
						unc = latestFile;
						Logger.Log(string.Format("Latest TFS Full backup logfile is {0}", unc));
					} catch (Exception ex) {
						Logger.LogException("PortalServicesText.getLog() TFS Logfile EXCEPTION: ", ex);
					}
				}

				try {
					//List<string> text = File.ReadLines("\\\\DBS\\files\\private\\Oracle\\tmp\\proc_autoUpd_quotes.log").Reverse().Take(50).ToList();
					List<string> text = File.ReadLines(unc).Reverse().Take(numLines).ToList();
					for (int i = text.Count - 1; i >= 0; i--) {
						result.AppendLine(text[i]);
					}
				} catch (Exception ex) {
					result.Clear();
					result.Append(ex.Message + "\r\nLog requested: " + whichLog);
					Logger.LogException("PortalServicesText.getLog() EXCEPTION: ", ex);
				}
			}

			byte[] resultBytes = Encoding.UTF8.GetBytes(result.ToString());
			WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
			Logger.Log("end");
			return new MemoryStream(resultBytes);
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/5/2018 
		/// </summary> 
		[OperationContract, WebGet]
		public Stream getTrollUptime() {
			/* On troll as boil, create pub/priv keys.  then add the pub key to authorized_keys (chmod 600).
             * copy id_rsa.pub to authorized_keys if you want.  Now grab the PRIVATE key and include it in app_code.  That's it.  If testing, note that
             * Van Dyke SecureCRT requires both keys be present on your client PC.
             * See HNET documents in TFS source control for more info.
             */
			//https://dzone.com/articles/sshnet 

			Logger.Log("start");
			string ret = "";

			try {
				string privateKeyFileName = Server.MapPath(@"~/App_Data") + "/boil_id_rsa";
				PrivateKeyFile pkf = new PrivateKeyFile(privateKeyFileName);

				using (var ssh = new SshClient("troll.hnet.local", 10222, "boil", pkf)) {
					ssh.Connect();
					var command = ssh.CreateCommand("uptime");
					ret = (string)command.Execute();
					//command = ssh.CreateCommand("df -v");
					//ret += (string)command.Execute();
					ssh.Disconnect();
				}
			} catch (Exception ex) {
				Logger.LogException("PortalServicesText.getTrollUptime() troll EXCEPTION", ex);
				ret = ex.Message;
			}

			byte[] resultBytes = Encoding.UTF8.GetBytes(ret);
			WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
			Logger.Log("end");
			return new MemoryStream(resultBytes);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/5/2018 
		/// </summary> 
		[OperationContract, WebGet]
		public Stream getFeed(int whichFeed) {

			string result = "";
			Logger.Log("getFeed " + whichFeed);

			try {
				MySqlConnection conn = new MySqlConnection();
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("select a.feedid, b.feedname, b.feedURL,  b.cacheFilePrefix from userfeedprefs a,      feedsmaster b where 	b.feedid = a .feedid	and a.username=@username and b.enabled='Y'  and b.feedid=@feedId", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.Parameters.AddWithValue("@feedId", whichFeed);
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				if (reader.Read()) {

					//URL's in[], eg [slashDot] are exceptions in the perl version that have their own
					//custom parsers.  In asp.net we can sometimes be more flexible and use a different feed URL offered
					//by the content provider.  Such is the case with slashDot.
					string feedUrl = (string)reader[2];
					Regex portMapPat = new Regex(@"\[(.*)\]");
					Match ma = portMapPat.Match(feedUrl);
					if (ma.Success) {
						string exName = ma.Groups[1].Value;
						feedUrl = (string)ConfigurationManager.AppSettings[exName + "_RssException"];
						Logger.Log("Doing  URL substitution for matched pattern " + exName + " to " + feedUrl);
					}
					result = goGetFeed(feedUrl, (string)reader[3]);

				}

			} catch (Exception ex) {
				result = ex.Message + " Error getFeed()";
				Logger.LogException("getFeed EXCEPTION: ", ex);
			}

			byte[] resultBytes = Encoding.UTF8.GetBytes(result);
			WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
			return new MemoryStream(resultBytes);
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		private
		/// <summary>
		/// Deprecated 3/5/2018 
		/// </summary> 
		string goGetFeed(string feedURL, string feedCacheName) {

			string cacheFileName = String.Format((string)ConfigurationManager.AppSettings["WorkDir"] + "/{0}.cache", feedCacheName);
			string ret = "";

			//7/17/2017: No too clever way to detect whether last fetch was unsuccessful 
			//due to (perhaps) "site down" or dns error, as the file will contain a 
			//short message.  In such cases we want to re-fetch regardless of cache file age.
			long fileLength = 101;
			try {
				fileLength = new System.IO.FileInfo(cacheFileName).Length;
			} catch { };


			try {
				int maxRssLinks = int.Parse(ConfigurationManager.AppSettings["maxRssLinks"]);
				var threshold = DateTime.Now.AddMinutes(-30);

				//fetch from cache OR if cache is stale, refresh and recache					
				if (File.GetLastWriteTime(cacheFileName) < threshold ||
					fileLength < 100) {
					Logger.Log("goGetFeed: " + cacheFileName + " is STALE or SMALL, so refetch and rebuild (" +
						File.GetLastWriteTime(cacheFileName).ToShortDateString() + " "
						+ File.GetLastWriteTime(cacheFileName).ToShortTimeString() + ") FileLength=" + fileLength.ToString());

					//get refreshed feed data
					ret = Fetch.Rss(feedURL, maxRssLinks);

					//cache it
					using (StreamWriter newTask = new StreamWriter(cacheFileName, false)) {
						newTask.WriteLine(ret);
					}

				} else {
					//read the cache file in 
					Logger.Log(string.Format("goGetFeed: " + cacheFileName + " is FRESH, FileLength={0}, Load from cache is okay.", fileLength.ToString()));
					Encoding encode = Encoding.GetEncoding("utf-8");
					using (StreamReader sr = new StreamReader(cacheFileName, encode)) {
						ret = sr.ReadToEnd();
					}
				}

			} catch (Exception ex) {
				ret = ex.Message + " Error goGetFeed()";
				Logger.LogException("goGetFeed EXCEPTION: ", ex);

			}
			return ret;
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/9/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getFeedMasterRec(int feedId) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			if (!User.IsInRole("Administrators")) {
				Logger.Log("IsInRole admin failed");
				return "[{\"status\": \"Error: Non-Admin Role\"}]";
			}

			Logger.Log("getFeedMasterRec(" + feedId.ToString() + ")");
			MySqlConnection conn = new MySqlConnection();
			var jsonSerialiser = new JavaScriptSerializer();
			List<FeedMasterItem> list = new List<FeedMasterItem>();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT feedid, feedName,  feedType, feedURL, cacheFilePrefix, enabled FROM feedsmaster where feedid=@feedid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@feedid", feedId);
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				reader.Read();
				FeedMasterItem fmi = new FeedMasterItem();
				fmi.feedid = feedId;
				fmi.feedName = (string)reader[1];
				fmi.feedType = (byte)reader[2];
				fmi.feedURL = (string)reader[3];
				fmi.cacheFilePrefix = (string)reader[4];
				fmi.enabled = (string)reader[5];
				list.Add(fmi);

			} catch (Exception ex) {
				Logger.LogException("getFeedMasterRec(" + feedId.ToString() + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("getFeedMasterRec(" + feedId.ToString() + "): Finished");
			return jsonSerialiser.Serialize(list);

		}


		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/9/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string editFeedMasterRec(int feedid, string feedName, byte feedType, string feedURL, string cacheFilePrefix, string enabled) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			if (!User.IsInRole("Administrators")) {
				Logger.Log("IsInRole admin failed");
				return "[{\"status\": \"Error: Non-Admin Role\"}]";
			}

			Logger.Log("editFeedMasterRec(" + feedid.ToString() + ")");
			MySqlConnection conn = new MySqlConnection();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("Update feedsmaster set feedName=@feedName, feedType=@feedType, feedURL=@feedURL, cacheFilePrefix=@cacheFilePrefix, enabled=@enabled where feedid=@feedid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@feedid", feedid);
				cmd.Parameters.AddWithValue("@feedName", feedName);
				cmd.Parameters.AddWithValue("@feedType", feedType);
				cmd.Parameters.AddWithValue("@feedURL", feedURL);
				cmd.Parameters.AddWithValue("@cacheFilePrefix", cacheFilePrefix);
				cmd.Parameters.AddWithValue("@enabled", enabled);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException("editFeedMasterRec(" + feedid.ToString() + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("editFeedMasterRec(" + feedid.ToString() + "): Finished");
			return getFeedMasterRec(feedid);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/9/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string delFeedMasterRec(int feedid) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			if (!User.IsInRole("Administrators")) {
				Logger.Log("IsInRole admin failed");
				return "[{\"status\": \"Error: Non-Admin Role\"}]";
			}

			MySqlConnection conn = new MySqlConnection();
			try {
				Logger.Log("delFeedMasterRec(" + feedid.ToString() + ") deleting feedmaster record");
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("delete from feedsmaster where feedid=@feedid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@feedid", feedid);
				cmd.ExecuteNonQuery();

				Logger.Log("delFeedMasterRec(" + feedid.ToString() + "), deleting any userfeedprefs for feedid");
				cmd = new MySqlCommand("delete from userfeedprefs where feedid=@feedid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@feedid", feedid);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException("delFeedMasterRec(" + feedid.ToString() + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("delFeedMasterRec(" + feedid.ToString() + "): Finished");
			return "[{\"status\": \"Success\"}]";

		}


		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/9/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string addFeedMasterRec(int feedid, string feedName, byte feedType, string feedURL, string cacheFilePrefix, string enabled) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			if (!User.IsInRole("Administrators")) {
				Logger.Log("IsInRole admin failed");
				return "[{\"status\": \"Error: Non-Admin Role\"}]";
			}

			Logger.Log("start");
			MySqlConnection conn = new MySqlConnection();
			var jsonSerialiser = new JavaScriptSerializer();
			List<FeedMasterItem> list = new List<FeedMasterItem>();
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
				cmd.Parameters.AddWithValue("@feedName", feedName);
				cmd.Parameters.AddWithValue("@feedType", feedType);
				cmd.Parameters.AddWithValue("@feedURL", feedURL);
				cmd.Parameters.AddWithValue("@cacheFilePrefix", cacheFilePrefix);
				cmd.Parameters.AddWithValue("@enabled", enabled);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException("addFeedMasterRec(" + nextFeedID.ToString() + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("addFeedMasterRec(" + nextFeedID.ToString() + "): Finished. Added okay.");
			return getFeedMasterRec((int)nextFeedID);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getUserLinksSections(int sectionid) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			Logger.Log("start");
			MySqlConnection conn = new MySqlConnection();
			var jsonSerialiser = new JavaScriptSerializer();
			List<userLinkSecItem> list = new List<userLinkSecItem>();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd;
				if (sectionid <= 0) {
					cmd = new MySqlCommand("select sectionid, orderby, sectionText,enabled, DATE_FORMAT(timestamp, '%m-%d-%Y %h:%i:%s') as timestamp from linkssection where username=@username order by orderby", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				} else {
					cmd = new MySqlCommand("select sectionid, orderby, sectionText,enabled, DATE_FORMAT(timestamp, '%m-%d-%Y %h:%i:%s') as timestamp from linkssection where username=@username and sectionid=@sectionid order by orderby", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@username", User.Identity.Name);
					cmd.Parameters.AddWithValue("@sectionid", sectionid);
				}
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				while (reader.Read()) {
					userLinkSecItem litem = new userLinkSecItem();
					litem.sectionid = (int)reader[0];
					litem.orderby = (int)reader[1];
					litem.sectionText = (string)reader[2];
					litem.enabled = (string)reader[3];
					litem.timestamp = (string)reader[4];
					list.Add(litem);
				}

			} catch (Exception ex) {
				Logger.LogException("getUserLinksSections: ", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("end");
			//System.Threading.Thread.Sleep(4000);
			return jsonSerialiser.Serialize(list);
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string delUserLinkSectionRec(int sectionid) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			Logger.Log("start");

			MySqlConnection conn = new MySqlConnection();
			try {
				Logger.Log("delUserLinkSectionRec(" + sectionid.ToString() + ") deleting Section record  record for user " + User.Identity.Name);
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("delete from linkssection where username=@username and sectionid=@sectionid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.Parameters.AddWithValue("@sectionid", sectionid);
				cmd.ExecuteNonQuery();

				Logger.Log("delUserLinkSectionRec(" + sectionid.ToString() + "), deleting links for sectionid " + sectionid.ToString() + " User=" + User.Identity.Name);
				cmd = new MySqlCommand("delete from linksdetail where username=@username and sectionid=@sectionid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.Parameters.AddWithValue("@sectionid", sectionid);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException("delUserLinkSectionRec(" + sectionid.ToString() + " User=" + User.Identity.Name + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("delUserLinkSectionRec(" + sectionid.ToString() + " User=" + User.Identity.Name + "): Finished");
			return "[{\"status\": \"Success\"}]";

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string editUserLinkSectionRec(int sectionid, string sectionText, int orderby, string enabled) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: xxAuthentication\"}]";
			}
			Logger.Log("start for sectionID " + sectionid + " User " + User.Identity.Name);

			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("Update linkssection set sectionText=@sectionText, orderby=@orderby, enabled=@enabled, username=@username, timestamp=CURRENT_TIMESTAMP where sectionid=@sectionid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@sectionid", sectionid);
				cmd.Parameters.AddWithValue("@sectionText", sectionText);
				cmd.Parameters.AddWithValue("@orderby", orderby);
				cmd.Parameters.AddWithValue("@enabled", enabled);
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException("(" + sectionid.ToString() + " User=" + User.Identity.Name + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("editFeedMasterRec(" + sectionid.ToString() + " User=" + User.Identity.Name + "): Finished");
			return getUserLinksSections(sectionid);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string addUserLinkSectionRec(int sectionid, string sectionText, int orderby, string enabled) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for sectionID " + sectionid + " User " + User.Identity.Name);

			int lastId = 0;
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("insert into linkssection(username, orderby, sectionText, enabled, timestamp) values(@username,@orderby, @sectionText, @enabled, CURRENT_TIMESTAMP)", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@sectionText", sectionText);
				cmd.Parameters.AddWithValue("@orderby", orderby);
				cmd.Parameters.AddWithValue("@enabled", enabled);
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.ExecuteNonQuery();

				cmd = new MySqlCommand("select max(sectionid) as lastId from linkssection where username=@username", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				reader.Read();
				lastId = (int)reader[0];
				Logger.Log("The new record's sectionid " + lastId.ToString());
				reader.Close();

			} catch (Exception ex) {
				Logger.LogException("addUserLinkSectionRec(" + sectionid.ToString() + " User=" + User.Identity.Name + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("addUserLinkSectionRec(" + lastId.ToString() + " User=" + User.Identity.Name + "): Finished");
			return getUserLinksSections(lastId);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getUserLinks(int linkid, int sectionid) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			Logger.Log("start");
			MySqlConnection conn = new MySqlConnection();
			var jsonSerialiser = new JavaScriptSerializer();
			List<userLinkItem> list = new List<userLinkItem>();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd;
				if (linkid <= 0) {
					cmd = new MySqlCommand("select linkid, sectionid, orderby, linkText,linkURL, subSectionText, hoverText, enabled, DATE_FORMAT(timestamp, '%m-%d-%Y %h:%i:%s') as timestamp, newwindow, ismenuitem from linksdetail where username=@username and sectionid=@sectionid order by orderby", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@sectionid", sectionid);
					cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				} else {
					cmd = new MySqlCommand("select linkid, sectionid, orderby, linkText,linkURL, subSectionText, hoverText, enabled, DATE_FORMAT(timestamp, '%m-%d-%Y %h:%i:%s') as timestamp, newwindow, ismenuitem from linksdetail where username=@username and  linkid=@linkid order by orderby", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@username", User.Identity.Name);
					cmd.Parameters.AddWithValue("@linkid", linkid);
				}

				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				while (reader.Read()) {
					userLinkItem litem = new userLinkItem();
					litem.linkid = (int)reader[0];
					litem.sectionid = (int)reader[1];
					litem.orderby = (int)reader[2];
					litem.linkText = (string)reader[3];
					litem.linkURL = (string)reader[4];
					litem.subSectionText = (string)reader[5];
					litem.hoverText = (string)reader[6];
					litem.enabled = (string)reader[7];
					litem.timestamp = (string)reader[8];
					litem.newwindow = (string)reader[9];
					litem.ismenuitem = (string)reader[10];
					list.Add(litem);
				}

			} catch (Exception ex) {
				Logger.LogException("getUserLinks: ", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("end, returning a list of size: " + list.Count());
			//System.Threading.Thread.Sleep(4000);
			return jsonSerialiser.Serialize(list);
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string addUserLinkRec(int linkid, int sectionid, string linkText, string linkURL, string subSectionText, string hoverText, uint orderby, string enabled, string newwindow, string ismenuitem) {

			//linkid is ignored here

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for sectionID " + sectionid + " User " + User.Identity.Name);

			int lastId = 0;
			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("insert into linksdetail( sectionid, orderby, username, linkText,linkURL, subSectionText, hoverText, enabled,newWindow, ismenuitem, timestamp) values(@sectionid, @orderby, @username, @linkText,@linkURL, @subSectionText, @hoverText, @enabled, @newWindow, @isMenuItem, CURRENT_TIMESTAMP)", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@sectionid", sectionid);
				cmd.Parameters.AddWithValue("@linkText", linkText);
				cmd.Parameters.AddWithValue("@linkURL", linkURL);
				cmd.Parameters.AddWithValue("@subSectionText", subSectionText);
				cmd.Parameters.AddWithValue("@hoverText", hoverText);
				cmd.Parameters.AddWithValue("@orderby", orderby);
				cmd.Parameters.AddWithValue("@enabled", enabled);
				cmd.Parameters.AddWithValue("@newWindow", newwindow);
				cmd.Parameters.AddWithValue("@isMenuItem", ismenuitem);
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				cmd.ExecuteNonQuery();

				cmd = new MySqlCommand("select max(linkid) as lastId from linksdetail where username=@username", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);
				MySqlDataReader reader;
				reader = cmd.ExecuteReader();
				reader.Read();
				lastId = (int)reader[0];
				Logger.Log("The new record's linkid is " + lastId.ToString());
				reader.Close();


			} catch (Exception ex) {
				Logger.LogException("addUserLinkRec(" + linkid.ToString() + " User=" + User.Identity.Name + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("addUserLinkRec(" + lastId.ToString() + " User=" + User.Identity.Name + "): Finished");
			return getUserLinks(lastId, 0);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string editUserLinkRec(int linkid, int sectionid, string linkText, string linkURL, string subSectionText, string hoverText, uint orderby, string enabled, string newwindow, string ismenuitem) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for linkID " + linkid + " User " + User.Identity.Name);

			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("update linksdetail set sectionid=@sectionid, orderby=@orderby, username=@username, linkText=@linkText,linkURL=@linkURL, subSectionText=@subSectiontext, hoverText=@hoverText, enabled=@enabled, ismenuitem=@isMenuItem, timestamp=CURRENT_TIMESTAMP, newwindow=@newWindow where linkid=@linkid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@linkid", linkid);
				cmd.Parameters.AddWithValue("@sectionid", sectionid);
				cmd.Parameters.AddWithValue("@linkText", linkText);
				cmd.Parameters.AddWithValue("@linkURL", linkURL);
				cmd.Parameters.AddWithValue("@subSectionText", subSectionText);
				cmd.Parameters.AddWithValue("@hoverText", hoverText);
				cmd.Parameters.AddWithValue("@orderby", orderby);
				cmd.Parameters.AddWithValue("@enabled", enabled);
				cmd.Parameters.AddWithValue("@newWindow", newwindow);
				cmd.Parameters.AddWithValue("@isMenuItem", ismenuitem);
				cmd.Parameters.AddWithValue("@username", User.Identity.Name);

				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException("editUserLinkRec(" + linkid.ToString() + " User=" + User.Identity.Name + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("editUserLinkRec(" + linkid.ToString() + " User=" + User.Identity.Name + "): Finished");
			return getUserLinks((int)linkid, 0);

		}


		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string delUserLinkRec(int linkid) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for linkID " + linkid + " User " + User.Identity.Name);

			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("delete from linksdetail where linkid=@linkid", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@linkid", linkid);
				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				Logger.LogException("delUserLinkRec(" + linkid.ToString() + " User=" + User.Identity.Name + ")", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("delUserLinkRec(" + linkid.ToString() + " User=" + User.Identity.Name + "): Finished");
			// return getUserLinks((int)linkid, 0);
			return "[{\"status\": \"Success\"}]";

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string resequenceLinksSections(string seq_sectionId, string seq_initialNum, string seq_incrementBy) {
			//sectionId is ignored.

			Logger.Log("Start");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			var jsonSerialiser = new JavaScriptSerializer();
			List<userLinkSecItem> list = new List<userLinkSecItem>();
			MySqlConnection conn = new MySqlConnection();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("resequenceLinksSections", conn);
				cmd.Prepare();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@xUserName", User.Identity.Name);
				cmd.Parameters.AddWithValue("@xStartingIdx", seq_initialNum);
				cmd.Parameters.AddWithValue("@xIncrementBy", seq_incrementBy);
				MySqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read()) {
					userLinkSecItem litem = new userLinkSecItem();
					litem.sectionid = (int)reader[0];
					litem.orderby = (int)reader[1];
					litem.sectionText = (string)reader[2];
					litem.enabled = (string)reader[3];
					litem.timestamp = (string)reader[4];
					list.Add(litem);
				}

			} catch (Exception ex) {
				Logger.LogException("error with resequence Section master", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}
			return jsonSerialiser.Serialize(list);

		}


		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string resequenceLinkSectionDetails(string seq_sectionId, string seq_initialNum, string seq_incrementBy) {

			Logger.Log("Start");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			var jsonSerialiser = new JavaScriptSerializer();
			List<userLinkItem> list = new List<userLinkItem>();
			MySqlConnection conn = new MySqlConnection();

			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("resequenceLinksDetail", conn);
				cmd.Prepare();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@xUserName", User.Identity.Name);
				cmd.Parameters.AddWithValue("@xSectionId", seq_sectionId);
				cmd.Parameters.AddWithValue("@xStartingIdx", seq_initialNum);
				cmd.Parameters.AddWithValue("@xIncrementBy", seq_incrementBy);
				MySqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read()) {
					userLinkItem litem = new userLinkItem();
					litem.linkid = (int)reader[0];
					litem.sectionid = (int)reader[1];
					litem.orderby = (int)reader[2];
					litem.linkText = (string)reader[3];
					litem.linkURL = (string)reader[4];
					litem.subSectionText = (string)reader[5];
					litem.hoverText = (string)reader[6];
					litem.enabled = (string)reader[7];
					litem.timestamp = (string)reader[8];
					litem.newwindow = (string)reader[9];
					litem.ismenuitem = (string)reader[10];
					list.Add(litem);
				}

			} catch (Exception ex) {
				Logger.LogException("error with resequence Section Details", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}
			return jsonSerialiser.Serialize(list);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/10/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string parseICalFile(string fileName) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for file " + fileName);

			string workDir = ConfigurationManager.AppSettings["WorkDir"];
			string workFileName = workDir + "/" + User.Identity.Name + "_" + fileName;
			var jsonSerialiser = new JavaScriptSerializer();
			List<ICal.ICalItem> list = new List<ICal.ICalItem>();

			try {

				Ical.Net.Calendar calendar = null;				
				using (StreamReader sr = new StreamReader(workFileName)) {
					calendar = Ical.Net.Calendar.Load(sr.ReadToEnd());
				}

				//calendar.AddTimeZone(new Ical.Net.VTimeZone("America/New_York"));

				for (int i = 0; i < calendar.Events.Count(); i++) {

					Ical.Net.CalendarComponents.CalendarEvent ev = calendar.Events[i];

					//DTSTART;TZID="America/New_York":20160928T190000
					//2016-09-28 07:00 PM
					//http://www.csharp-examples.net/string-format-datetime/

					ICal.ICalItem item = new ICal.ICalItem {
						uid = ev.Uid,
						summary = ev.Summary,
						startDate = String.Format("{0:yyyy-MM-dd hh:mm tt}", ev.DtStart.AsSystemLocal),
						endDate = String.Format("{0:yyyy-MM-dd hh:mm tt}", ev.DtEnd != null ? ev.DtEnd.AsSystemLocal : ev.DtStart.AsSystemLocal),
						location = ev.Location,
						description = ev.Description
					};
					list.Add(item);
					// Logger.Log(string.Format("{0}: {1} {2}", item.uid, item.startDate, item.description));
				}
				Logger.Log("Build iCal event list of size " + calendar.Events.Count());

			} catch (System.IndexOutOfRangeException ex) {
				Logger.LogException("parseICalFile: Exception ", ex);
				return "[{\"status\": \"Error: Bad ICal File\"}]";
			} catch (Exception ex) {
				Logger.LogException("parseICalFile: Exception ", ex);
				return "[{\"status\": \"Exception:" + ex + "\"}]";
			}

			return jsonSerialiser.Serialize(list);

		}


		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string dirNameChange(string whichDir, string newDirName) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for dir " + whichDir + " TO " + newDirName);

			try {
				DirectoryInfo tOldDir = new DirectoryInfo(Context.Server.MapPath("~/imagebase/thumb/" + whichDir));
				DirectoryInfo fOldDir = new DirectoryInfo(Context.Server.MapPath("~/imagebase/full/" + whichDir));

				System.IO.Directory.Move(tOldDir.FullName, tOldDir.Parent.FullName + "/" + newDirName);
				Logger.Log("rename of " + tOldDir.FullName + " probably worked!");
				System.IO.Directory.Move(fOldDir.FullName, fOldDir.Parent.FullName + "/" + newDirName);
				Logger.Log("rename of " + fOldDir.FullName + " probably worked too!");


				//Now rename the imbasedirperm record if any
				MySqlConnection conn = new MySqlConnection();
				try {
					Logger.Log(string.Format("sql: update imbasedirperm set dirname={0} where dirname={1}", newDirName, whichDir));
					conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
					conn.Open();
					MySqlCommand cmd = new MySqlCommand("update imbasedirperm set dirname=@newDirName where dirname=@whichDir", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@newDirName", newDirName);
					cmd.Parameters.AddWithValue("@whichDir", whichDir);
					cmd.ExecuteNonQuery();
				} catch (Exception ex) {
					Logger.LogException("imbasedirperm: rename dir exception", ex);
					//were not going to consider this fatal to the function
				} finally {
					conn.Close();
				}

			} catch (Exception ex) {
				Logger.LogException("dirNameChange: Exception ", ex);
				return "[{\"status\": \"Exception:" + ex + "\"}]";
			}

			Logger.Log("returning success status");
			return "[{\"status\": \"Success\"}]";
		}


		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string deleteImage(string whichImage) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for " + whichImage);

			try {
				//This function assumes whichImage is the file in the "Full" directory, and
				//will delete it and its related thumbnail file
				DirectoryInfo fullFile = new DirectoryInfo(Context.Server.MapPath("~" + whichImage));
				File.Delete(fullFile.FullName);
				Logger.Log("Deleted" + fullFile.FullName);

				string thumbFileName = fullFile.FullName;
				thumbFileName = thumbFileName.Replace("\\imagebase\\full\\", "\\imagebase\\thumb\\");
				File.Delete(thumbFileName);

			} catch (Exception ex) {
				Logger.LogException("deleteImage: Exception ", ex);
				return "[{\"status\": \"Exception:" + ex + "\"}]";
			}

			Logger.Log("returning success status");
			return "[{\"status\": \"Success\"}]";
		}


		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string createImageDir(string newDirName) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for " + newDirName);

			DirectoryInfo di = new DirectoryInfo(Context.Server.MapPath("~\\imagebase\\full\\" + newDirName));
			if (System.IO.Directory.Exists(di.FullName)) {
				Logger.Log("error, returning dir already exists");
				return "[{\"status\": \"Error: Directory already exists\"}]";
			}

			try {
				Directory.CreateDirectory(di.FullName);
			} catch (Exception ex) {
				Logger.LogException("createImageDir: Exception ", ex);
				return "[{\"status\": \"Exception:" + ex + "\"}]";
			}

			Logger.Log("returning success status");
			return "[{\"status\": \"Success\"}]";
		}

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string deleteImageDir(string dirName) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for " + dirName);

			DirectoryInfo di = new DirectoryInfo(Context.Server.MapPath("~\\imagebase\\full\\" + dirName));
			if (!System.IO.Directory.Exists(di.FullName)) {
				Logger.Log("error, returning dir does not exist");
				return "[{\"status\": \"Error: Directory does not exist\"}]";
			}

			try {
				Directory.Delete(di.FullName, true); //recursive
			} catch (Exception ex) {
				Logger.LogException("deleteImageDir: Exception ", ex);
				return "[{\"status\": \"Exception:" + ex + "\"}]";
			}

			//now delete the thumb dir recurively if it exists
			string thumbDir = di.FullName;
			thumbDir = thumbDir.Replace("\\imagebase\\full\\", "\\imagebase\\thumb\\");
			if (System.IO.Directory.Exists(thumbDir)) {
				Logger.Log("Found thumb dir");
				try {
					Directory.Delete(thumbDir, true); //recursive
				} catch (Exception ex) {
					Logger.LogException("deleteImageDir: Thumbdir Exception ", ex);
					return "[{\"status\": \"Exception on thumbdir" + ex + "\"}]";
				}
			}

			//Now delete the imbasedirperm record if any
			MySqlConnection conn = new MySqlConnection();
			try {
				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("delete from imbasedirperm where dirname=@dirname", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@dirname", dirName);
				cmd.ExecuteNonQuery();
			} catch (Exception ex) {
				Logger.LogException("imbasedirperm: delete dir exception", ex);
				//were not going to consider this fatal to the function
			} finally {
				conn.Close();
			}

			Logger.Log("returning success status");
			return "[{\"status\": \"Success\"}]";
		}


		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string imageDirSetPerm(string dirName, string hasPublicAccess) {

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			Logger.Log("start for dirName " + dirName + " hasPublicAccess= " + hasPublicAccess);

			MySqlConnection conn = new MySqlConnection();
			try {

				conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
				conn.Open();
				MySqlCommand cmd;
				if (hasPublicAccess.Equals("Y")) {

					Logger.Log("preparing statement for insert/update");
					cmd = new MySqlCommand("insert into imbasedirperm values(@userName,@dirName,'Y', now()) ON DUPLICATE KEY UPDATE " +
						"userName=@userName, publicaccess='Y', updated_ts=now()", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@username", User.Identity.Name);
					cmd.Parameters.AddWithValue("@dirName", dirName);

				} else {
					Logger.Log("preparing statement for deletion");
					cmd = new MySqlCommand("delete from imbasedirperm where dirName=@dirName", conn);
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@dirName", dirName);
				}

				cmd.ExecuteNonQuery();

			} catch (Exception ex) {
				//Logger.LogException("editUserLinkRec(" + linkid.ToString() + " User=" + User.Identity.Name + ")", ex);
				Logger.LogException("imageDirSetPerm  sql exception", ex);
				return "[{\"status\": \"Error:" + ex + "\"}]";

			} finally {
				conn.Close();
			}

			Logger.Log("returning success status");
			return "[{\"status\": \"Success\"}]";

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/5/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getTFSBuildStatus(string projectName, string buildName) {

			Logger.Log(string.Format("begin projectName='{0}' buildName='{1}'", projectName, buildName));

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			TFBuildResultItem bri = TeamFoundation.GetBuildStatus(projectName, buildName);
			if (bri.exception != null) {
				Logger.Log("sending JSON error since TeamFoundation.getBuildStatus returned an exception");
				return "[{\"status\": \"Error: Exception calling tfs server\"}]";
			}

			var jsonSerialiser = new JavaScriptSerializer();
			Logger.Log(string.Format("Returning a successfully retrieved latest build record for projectName='{0}' buildName='{1}'", projectName, buildName));
			return jsonSerialiser.Serialize(bri);
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/5/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getTFSSourceFileNames(string sourcePathName) {

			Logger.Log(string.Format("begin sourceFilePath='{0}'", sourcePathName));

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			List<string> fileList_s = TeamFoundation.fileList(sourcePathName, new List<string> { "/packages/", "/scripts/scripts/", "/bin/", "/obj/", "/images/", "/fonts/" });
			List<TFSourceItem> outList = new List<TFSourceItem>();
			foreach (var s in fileList_s) {
				TFSourceItem fi = new TFSourceItem(s);
				outList.Add(fi);
			}

			var jsonSerialiser = new JavaScriptSerializer();
			Logger.Log("Returning list of source files");
			return jsonSerialiser.Serialize(outList);

		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/5/2018 
		/// </summary> 
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getTFSSourceFileContents(string sourceFileName) {

			Logger.Log(string.Format("begin sourceFileName='{0}'", sourceFileName));

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			if (!User.IsInRole("Administrators")) {
				Logger.Log("IsInRole admin failed");
				return "[{\"status\": \"Error: Non-Admin Role\"}]";
			}

			string sourceContents = TeamFoundation.getSource(sourceFileName);
			sourceContents = Server.HtmlEncode(sourceContents); //7.9.2017: added this to handle code w/ html in it (.aspx pages, etc.)
			TFSourceItem si = new TFSourceItem(sourceFileName, sourceContents);
			
			var jsonSerialiser = new JavaScriptSerializer();
			Logger.Log("Returning source file contents");
			return jsonSerialiser.Serialize(si);

		}


		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/13/2018 
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string plexCopyToHome(bool clearDestFirst, string plexPathID, string fileCount, string selOption) {

			//BUG: If 2 or more run this at the same time, there will be errors.  Not a major
			//problem because errors are handled well, and retries will fix.

			//System.Threading.Thread.Sleep(5000);
			// return "[{\"status\": \"Success\"}]";

			Logger.Log(string.Format("begin clearDestFirst='{0}' plexPathID='{1}' fileCount='{2}", clearDestFirst, plexPathID, fileCount));
			var jsonSerialiser = new JavaScriptSerializer();

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return jsonSerialiser.Serialize(new { status = "Error: Authentication"});
			}

			if (!User.IsInRole("Administrators")) {
				Logger.Log("IsInRole admin failed");
				return jsonSerialiser.Serialize(new { status = "Error: Non-Admin Role" });
			}

			int _fileCount;
			try {
				_fileCount = int.Parse(fileCount);

			} catch (Exception ex) {
				Logger.LogException("plexCopyToHome: Parse Int Exception ", ex);
				return jsonSerialiser.Serialize(new { status = "Error: data type "+ex });
			}

			string toPath = ConfigurationManager.AppSettings["PLEX_RAND_DESTPATH"];
			string whichEnv = ConfigurationManager.AppSettings["ENVIRONMENT"];
			string fromPath = "";
			bool isHome = false;

			//Lookup the source directory's path from the xml file
			try {
				var xDoc = new XmlDocument();
				xDoc.Load(Server.MapPath("~/App_Data/PlexPaths.xml"));
				XmlNode node = xDoc.SelectSingleNode(string.Format("//plexPath[@ID='{0}' and @enabled='TRUE']", plexPathID));  //should be unique, s ignore dups
				fromPath = node[whichEnv].InnerText;
				isHome = node["isHome"].InnerText.ToLower().Equals("true");

			} catch (Exception ex) {
				Logger.LogException("plexCopyToHome: XML Exception ", ex);
				return jsonSerialiser.Serialize(new { status = "Error Exception on XML "+ex });
			}
			Logger.Log(string.Format("Got the source path from XML. isHome={0}", isHome));

			//Purge the destination directory if requested OR if we are restoring Home movies
			if (clearDestFirst || isHome) {
				try {
					Logger.Log(string.Format("Starting clean dir.   cleanDestFirst={0}, isHome={1}", clearDestFirst, isHome));
					DirectoryInfo toDir = new DirectoryInfo(toPath);
					foreach (FileInfo fi in toDir.GetFiles()) {
						fi.Delete();
					}
					Logger.Log("Executed clear dir operation");
				} catch (Exception ex) {
					Logger.LogException("plexCopyToHome: Purge destination exception ", ex);
					return jsonSerialiser.Serialize(new { status = "Error: Error Purging Destination " + ex });
				}

			}

			try {

				//Get a list of files from the source dir
				DirectoryInfo Dir = new DirectoryInfo(fromPath);
				FileInfo[] fList = Dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
				Logger.Log(string.Format("there are {0} files in the source path", fList.Count()));

				if (isHome) {

					//Do the copy
					Logger.Log(string.Format("RESTORING Home Dir count={0}", fList.Count()));
					for (int i = 0; i < fList.Count(); i++) {
						FileInfo file = fList[i];
						File.Copy(file.FullName, toPath + "/" + file.Name, true);
					}
					Logger.Log("RESTORE Home dir DONE.");
				} else {

					if (selOption.ToLower().Equals("newest")) {
						int maxFiles = _fileCount > fList.Count() ? fList.Count() : _fileCount;
						fList = Directory.GetFiles(fromPath)
							.Select(x => new FileInfo(x))
							.OrderByDescending(x => x.LastWriteTime)
							.Take(maxFiles)
							.ToArray();

						//Do the copy
						for (int i = 0; i < maxFiles; i++) {
							FileInfo file = fList[i];
							File.Copy(file.FullName, toPath + "/" + file.Name, true);
						}

					} else if (selOption.ToLower().Equals("random")) {

						//create an array of random indexes of flist
						int[] randArr = new int[_fileCount];
						Random random = new Random();
						int maxFiles = _fileCount > fList.Count() ? fList.Count() : _fileCount;
						Logger.Log(string.Format("maxFiles to copy: {0}", maxFiles));
						for (int i = 0; i < maxFiles; i++) {
							int rn = random.Next(0, (fList.Count() - 1));
							randArr[i] = rn;
							Logger.Log(string.Format("Rand (size={0}): {1}={2}", maxFiles, i, rn));
							//todo, prevent dup rn from being used.  For now, we simply overwrite and wind up short a file or two.
						}

						//Do the copy
						for (int i = 0; i < maxFiles; i++) {
							FileInfo file = fList[randArr[i]];
							File.Copy(file.FullName, toPath + "/" + file.Name, true);
						}

					} else {
						//unknown selOption value
						Logger.Log($"plexCopyToHome: unknown selOption '{selOption}'");
						return jsonSerialiser.Serialize(new { status = "Error: Error Purging Destination " + selOption });
					}
					
				}

			} catch (Exception ex) {
				Logger.LogException("plexCopyToHome: Error during last phase ", ex);
				return jsonSerialiser.Serialize(new { status = "Error: last phase " + ex});
			}

			if (isHome) {
				plexSectionRefresh("2");
			}

			Logger.Log("returning success status");						
			return jsonSerialiser.Serialize(	new {	status = "Success"	}	);			

		}


		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/13/2018 
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string plexSectionRefresh(string sectionNum) {

			Logger.Log(string.Format("begin sectionNum='{0}", sectionNum));
			var jsonSerialiser = new JavaScriptSerializer();

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return jsonSerialiser.Serialize(new { status = "Error: Authentication" });
			}

			if (!User.IsInRole("Administrators")) {
				Logger.Log("IsInRole admin failed");
				return jsonSerialiser.Serialize(new { status = "Error: Non-Admin Role" });
			}

			string plexUrl = ConfigurationManager.AppSettings["PLEX_URL_METAREFRESH"];
			string plexSec = ConfigurationManager.AppSettings["PLEX_HOMESECTION_NUM"];
			string plexToken = ConfigurationManager.AppSettings["PLEX_TOKEN"];

			Fetch.CallRest(string.Format(plexUrl, sectionNum, plexToken), "");

			Logger.Log("returning success status");
			return jsonSerialiser.Serialize(new { status = "Success" });
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/6/2018 
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getVMState(string vmName) {

			string ret = "";
			Logger.Log(string.Format("begin vmName='{0}", vmName));

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			if (!User.IsInRole("Administrators")) {
				Logger.Log("IsInRole admin failed");
				return "[{\"status\": \"Error: Non-Admin Role\"}]";
			}

			try {

				VMWare vmware = new VMWare(string.Empty, string.Empty);  //deprecated args added
				if (vmware.Connect() != 0) {
					throw new Exception("Couldn't connect to ESXI server");
				}
				VMState vmstate = vmware.GetVMStateByName(vmName);
				Logger.Log("Returning VMState as " + vmstate.ToString());
				ret = "[{\"vmState\": \"" + vmstate.ToString() + "\"}]";
			} catch (Exception ex) {
				Logger.LogException("Get VMState failed", ex);
				return "[{\"status\": \"Error: " + ex.Message + "\"}]";
			}

			Logger.Log("ending normally");
			return ret;
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/6/2018 
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string setVMState(string vmName, string setToState) {

			string ret = "";
			Logger.Log(string.Format("begin vmName='{0} setto={1}", vmName, setToState.ToString()));

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			if (!User.IsInRole("Administrators")) {
				Logger.Log("IsInRole admin failed");
				return "[{\"status\": \"Error: Non-Admin Role\"}]";
			}

			try {

				VMWare vmware = new VMWare("","");//deprecated
				if (vmware.Connect() != 0) {
					throw new Exception("Couldn't connect to ESXI server");
				}

				VMState res = VMState.Unknown;
				if (setToState == "PowerOn") {
					Logger.Log("Sending STARTUP request for " + vmName);
					res = vmware.VMStartup(vmName);
				} else if (setToState == "Shutdown") {
					Logger.Log("Sending SHUTDOWN request for " + vmName);
					res = vmware.VMShutdown(vmName);
				}
				ret = "[{\"vmState\": \"" + res.ToString() + "\"}]";

			} catch (Exception ex) {
				Logger.LogException("Set VMState failed", ex);
				return "[{\"status\": \"Error: " + ex.Message + "\"}]";
			}

			Logger.Log("ending normally");
			return ret;
		}


		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getSoftwareDBCombinedCdAppList(string appTypeFilter) {

			Logger.Log("Start");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			List<CombinedCdAppItem> theList = SoftwareDB.getCombinedCdAppList(appTypeFilter);
			var jsonSerialiser = new JavaScriptSerializer();
			Logger.Log("Ending normally and returning list");
			return jsonSerialiser.Serialize(theList.OrderBy(x => x.appName.ToLower()).ToList());

		}


		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getSoftwareDBCompCdNameList() {

			Logger.Log("Start");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			List<cdCompNameItem> theList = SoftwareDB.getcdCompNameList();
			var jsonSerialiser = new JavaScriptSerializer();
			Logger.Log("Ending normally and returning list");
			return jsonSerialiser.Serialize(theList.OrderBy(x => x.cdName.ToLower()).ToList());

		}


		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getSoftwareDBIsoCdList(string appTypeFilter) {

			Logger.Log("Start");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			List<cdIsoItem> theList = SoftwareDB.getIsoCdList(appTypeFilter);
			var jsonSerialiser = new JavaScriptSerializer();
			Logger.Log("Ending normally and returning list");
			//return jsonSerialiser.Serialize(theList.OrderBy(x => x.cdName).ToList());
			return jsonSerialiser.Serialize(theList);

		}


		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getSoftwareDBCompCdAppList(string cdName, string appTypeFilter) {
			Logger.Log("Start");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			List<cdCompItem> theList = SoftwareDB.getCdCompAppList(cdName, appTypeFilter);
			var jsonSerialiser = new JavaScriptSerializer();
			Logger.Log("Ending normally and returning list");
			return jsonSerialiser.Serialize(theList.OrderBy(x => x.appName.ToLower()).ToList());
			//return jsonSerialiser.Serialize(theList);
		}


		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getSoftwareDBAppTypeList() {
			Logger.Log("Start");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			List<appTypeItem> theList = SoftwareDB.getAppTypeList();
			var jsonSerialiser = new JavaScriptSerializer();
			Logger.Log("Ending normally and returning list");
			//Logger.Log("LIST:"+jsonSerialiser.Serialize(theList));
			return jsonSerialiser.Serialize(theList);
		}

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getSoftwareDBSearchResults(string searchTerm) {

			Logger.Log("Start");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			List<CombinedCdAppItem> theList = SoftwareDB.getSearchResults(searchTerm);
			var jsonSerialiser = new JavaScriptSerializer();
			Logger.Log("Ending normally and returning list");
			return jsonSerialiser.Serialize(theList.OrderBy(x => x.appName.ToLower()).ToList());

		}



		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string writeSoftwareDBAppType(string type_id_orig, string type_id, string description, string longDescription, string dbActionType) {

			Logger.Log("Start");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			try {

				appTypeItem buf = new appTypeItem();
				buf.type_id = type_id;
				buf.description = description;
				buf.longDescription = longDescription;

				swdbActionType act = (swdbActionType)Enum.Parse(typeof(swdbActionType), dbActionType);
				SoftwareDB.writeAppType(type_id_orig, buf, act);
			} catch (SqlException ex) {
				if (ex.Number == 2627) {
					Logger.Log("***Returning duplicate Key Error in status field to caller");
					return "[{\"status\": \"Error: Duplicate Record!\"}]";
				}
			} catch (Exception ex) {
				Logger.Log("Exception");
				return "[{\"status\": \"Error: " + ex.Message + "\"}]";
			}

			return "[{\"status\": \"Success\"}]";

		}

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string writeSoftwareDBAppRec(string dbActionType, string filetype,
			string id, string appName, string cdName, string volumeName, string appType,
			string appDate, string numFiles, string numBytes, string numSubDirs,
			string path, string fileTypes, string @private, string notes) {

			Logger.Log("Start.  Action Type: " + dbActionType + " recordType=" + filetype);

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}

			try {
				swdbActionType action = (swdbActionType)Enum.Parse(typeof(swdbActionType), dbActionType);

				if (filetype.ToLower().Equals("cdiso")) {
					cdIsoItem buf = new cdIsoItem();
					buf.id = int.Parse(id);
					buf.appName = appName;
					buf.appType = appType;
					buf.cdName = cdName;
					buf.notes = notes;
					buf.volumeName = volumeName;
					string ret = SoftwareDB.writeIsoItem(buf, action);
					if (ret != "") { //Create and Delete actions return recordId info
						Logger.Log("returning success with " + ret);
						return "[{\"status\": \"Success: " + ret + "\"}]";
					}
				} else {
					//must be cdComp
					cdCompItem buf = new cdCompItem();
					buf.id = int.Parse(id);
					buf.appName = appName;
					buf.appType = appType;
					buf.cdName = cdName;
					buf.notes = notes;
					buf.volumeName = volumeName;

					buf.numBytes = int.Parse(numBytes);
					buf.numFiles = int.Parse(numFiles);
					buf.numSubDirs = int.Parse(numSubDirs);
					buf.fileTypes = fileTypes;
					buf.path = path;
					buf.appDate = Convert.ToDateTime(appDate);
					buf.@private = @private != null;

					//todo
					string ret = SoftwareDB.writecdCompItem(buf, action);

					if (ret != "") { //Create and Delete actions return recordId info
						Logger.Log("returning success with " + ret);
						return "[{\"status\": \"Success: " + ret + "\"}]";
					}

				}
			} catch (SqlException ex) {
				if (ex.Number == 2627) {
					Logger.Log("***Returning duplicate Key Error in status field to caller");
					return "[{\"status\": \"Error: Duplicate Record!\"}]";
				}
			} catch (Exception ex) {
				Logger.Log("Exception");
				return "[{\"status\": \"Error: " + ex.Message + "\"}]";
			}

			Logger.Log("returning Success with no further info");
			return "[{\"status\": \"Success\"}]";
		}


		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/12/2018 
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
		public string getVaultRec(string uuid) {

			Logger.Log($"Start uuid={uuid}");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				return "[{\"status\": \"Error: Authentication\"}]";
			}
			try {
				string dbpath = (string)ConfigurationManager.AppSettings["KEEPASSPATH"];
				KeePass kp = new KeePass(dbpath, Global.PortalSettings.KeePassDBPassword);
				PwEntry entry = kp.FindByUUID(uuid);

				if (entry != null) {

					KeePass.VaultEntryItem vei = new KeePass.VaultEntryItem {
						uuid = uuid,
						Title = entry.Strings.ReadSafe("Title"),
						Notes = entry.Strings.ReadSafe("Notes"),
						UserName = entry.Strings.ReadSafe("UserName"),
						URL = entry.Strings.ReadSafe("URL")
					};

					//several varieties of passwords, all encrypted or masked.
					string encryptionKey = Global.PortalSettings.KeePassEncrptionKey;
					byte[] bs = Crypto.EncryptStringToBytes(entry.Strings.ReadSafe("Password"), Encoding.UTF8.GetBytes(encryptionKey), Encoding.UTF8.GetBytes(encryptionKey));
					vei.PasswordEncrypted = bs;
					vei.PasswordEncryptedBase64 = Convert.ToBase64String(bs);

					var jsonSerialiser = new JavaScriptSerializer();
					Logger.Log($"Success. KeePass returned record for {uuid} with UserName={vei.UserName}");
					return jsonSerialiser.Serialize(vei);
				}

			} catch (Exception ex) {
				Logger.LogException("Error creating return data", ex);
				return "[{\"status\": \"Error: Service Exception\"}]";
			}

			return "[{\"status\": \"Error: KeePass trouble\"}]";
		}

		[Obsolete("Deprecated. Please use RESTful api instead")]
		/// <summary>
		/// Deprecated 3/6/2018 
		/// </summary>
		[OperationContract, WebGet]
		public System.IO.Stream getUserCalendarHtml(int _monthNo, int _yearNo) {
			
			Logger.Log($"Start monthNo={_monthNo}, yearNo={_yearNo}");

			string mySqlDateFormat = "yyyy-MM-dd";

			DateTime rDate = DateTime.Today;
			DateTime todaysDate = DateTime.Today;

			int todayDay = rDate.Day;
			int monthNo = (_monthNo > 0 && _monthNo < 13) ? _monthNo : rDate.Month;
			int yearNo = (_yearNo > 2000) ? _yearNo : rDate.Year;
			rDate = new DateTime(yearNo, monthNo, todayDay);
			string monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(rDate.Month);

			DateTime firstDayOfMonth = new DateTime(yearNo, monthNo, 1);
			DateTime LastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

			DateTime lastMonth = rDate.AddMonths(-1);
			DateTime nextMonth = rDate.AddMonths(+1);

			Logger.Log($"calendar's date is {monthNo}/{todayDay}/{yearNo}");

			if (!User.Identity.IsAuthenticated) {
				Logger.Log("IsAuthenticated failed");
				byte[] rb = Encoding.UTF8.GetBytes("Not authenticated");
				WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
				return new MemoryStream(rb);
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append($@"
						<table id='calTable' class='table-condensed table-bordered table-striped'>
						<thead>							
							<tr>
								<th onclick='loadCalendar({lastMonth.Month},{lastMonth.Year})' class='navbar-default' style='color:white; cursor: pointer; text-align:center'><i class='glyphicon glyphicon-backward'></i></th>
								<th colspan='5' style='width: 250px;text-align:center'><a href='{System.Web.VirtualPathUtility.ToAbsolute("~/Private/Calendar.aspx")}?InitialDate={firstDayOfMonth.ToString(mySqlDateFormat)}'>{monthName} {rDate.Year} </a></th>
								<th onclick='loadCalendar({nextMonth.Month},{nextMonth.Year})' class='navbar-default' style='color:white;  cursor: pointer; text-align:center'><i class='glyphicon glyphicon-forward'></i></th>
							</tr>
							<tr>
								<th>Su</th><th>Mo</th><th>Tu</th><th>We</th><th>Th</th><th>Fr</th><th>Sa</th>
							</tr>
						</thead>
						<tbody>
						<tr>
			");

			string eventsJson = getCalEvents(firstDayOfMonth.ToString(mySqlDateFormat), LastDayOfMonth.ToString(mySqlDateFormat));
			List<ICal.EventItem> eventList = JsonConvert.DeserializeObject<List<ICal.EventItem>>(eventsJson);			
			List<string> cells = new List<string>(42);
			int currDayNo = 1;
			for (int i = 0; i < 42; i++) {

				int dayOfWeekIdx = i % 7;
				try {

					DateTime currDay = new DateTime(rDate.Year, rDate.Month, currDayNo);
					string @class = "class='calCell'";

					if ((int)currDay.DayOfWeek == dayOfWeekIdx) {

						string hover = "";
						ICal.EventItem todaysEvent = eventList.FirstOrDefault(x => x.start.Equals(currDay.ToString(mySqlDateFormat)));
						if (todaysEvent != null) {
							//Logger.Log($"event found for {todaysEvent.start}: {todaysEvent.title}");							
							if (currDay == todaysDate)
								@class = "class='calCell calCellHighlight calCellToday'";
							else
								@class = "class='calCell calCellHighlight'";

							hover = $"title='{todaysEvent.title.Replace("\'", "\\'")}'";
						} else {
							if (currDay == todaysDate)
								@class = "class='calCell calCellHighlight calCellToday'";
						}

						builder.AppendLine($"<td  class='calCell' onclick=' launchCalEditDlg(\"{currDay.ToString(mySqlDateFormat)}\");'><a {@class} href='#' {hover}>{currDayNo}</a></td>");
						currDayNo++;

					} else {
						builder.AppendLine($"<td class='muted'>&nbsp;</td>");
					}

				} catch (Exception) {
					builder.AppendLine($"<td class='muted'>&nbsp;</td>");
				}

				if (dayOfWeekIdx == 6) {
					builder.AppendLine("</tr><tr>");
				}
			}
			builder.AppendLine($@"</tr></tbody></table>");

			Logger.Log("Ending");
			byte[] resultBytes = Encoding.UTF8.GetBytes(builder.ToString());
			WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
			return new MemoryStream(resultBytes);
		}

	}

}