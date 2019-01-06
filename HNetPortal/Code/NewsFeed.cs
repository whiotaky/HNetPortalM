using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WSHLib;
using WSHLib.Network;

namespace HNetPortal {
	public static class NewsFeed {

		public static MemoryStream Get(int whichFeed) {

			string result = "";
			Logger.Log("getFeed " + whichFeed);

			try {
				MySqlConnection conn = new MySqlConnection {
					ConnectionString = Global.getConnString(SupportedDBTypes.MySql)
				};
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("select a.feedid, b.feedname, b.feedURL,  b.cacheFilePrefix from userfeedprefs a,      feedsmaster b where 	b.feedid = a .feedid	and a.username=@username and b.enabled='Y'  and b.feedid=@feedId", conn);
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
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
			//WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
			return new MemoryStream(resultBytes);
		}

		private static string goGetFeed(string feedURL, string feedCacheName) {
			
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

	}
}