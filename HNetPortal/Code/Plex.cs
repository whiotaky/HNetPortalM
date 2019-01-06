using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Xml;
using WSHLib;
using WSHLib.Network;

namespace HNetPortal {
	public static class Plex {

		public class Params {
			public bool clearDestFirst { get; set; }
			public string plexPathID { get; set; }
			public string fileCount { get; set; }
			public string selOption { get; set; }
		}

		public static string CopyToHome(Params @params) {

			//BUG: If 2 or more run this at the same time, there will be errors.  Not a major
			//problem because errors are handled well, and retries will fix.

			//System.Threading.Thread.Sleep(5000);
			// return "[{\"status\": \"Success\"}]";

			bool clearDestFirst =@params.clearDestFirst;
			string plexPathID = @params.plexPathID;
			string fileCount = @params.fileCount;
			string selOption = @params.selOption;

			Logger.Log(string.Format("begin clearDestFirst='{0}' plexPathID='{1}' fileCount='{2}", clearDestFirst, plexPathID, fileCount));


			int _fileCount;
			try {
				_fileCount = int.Parse(fileCount);

			} catch (Exception ex) {
				Logger.LogException("plexCopyToHome: Parse Int Exception ", ex);
				return $"Error: data type";
			}

			string toPath = ConfigurationManager.AppSettings["PLEX_RAND_DESTPATH"];
			string whichEnv = ConfigurationManager.AppSettings["ENVIRONMENT"];
			string fromPath = "";
			bool isHome = false;

			//Lookup the source directory's path from the xml file
			try {
				var xDoc = new XmlDocument();
				xDoc.Load(HttpContext.Current.Server.MapPath("~/App_Data/PlexPaths.xml"));
				XmlNode node = xDoc.SelectSingleNode(string.Format("//plexPath[@ID='{0}' and @enabled='TRUE']", plexPathID));  //should be unique, s ignore dups
				fromPath = node[whichEnv].InnerText;
				isHome = node["isHome"].InnerText.ToLower().Equals("true");

			} catch (Exception ex) {
				Logger.LogException("plexCopyToHome: XML Exception ", ex);
				return $"Error: XML parse";
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
					return "Error: Purge destination exception ";
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
						return $"Error: unknown selOption: '{selOption}'";
					}

				}

			} catch (Exception ex) {
				Logger.LogException("plexCopyToHome: Error during last phase ", ex);
				return "Error: during last phase ";
			}

			if (isHome) {
				SectionRefresh("2");
			}

			Logger.Log("returning success status");
			return "Success: It worked";

		}


		public static string SectionRefresh(string sectionNum) {

			Logger.Log(string.Format("begin sectionNum='{0}", sectionNum));


			string plexUrl = ConfigurationManager.AppSettings["PLEX_URL_METAREFRESH"];
			string plexSec = ConfigurationManager.AppSettings["PLEX_HOMESECTION_NUM"];
			string plexToken = ConfigurationManager.AppSettings["PLEX_TOKEN"];

			Fetch.CallRest(string.Format(plexUrl, sectionNum, plexToken), "");

			Logger.Log("returning success status");
			return "Success";// jsonSerialiser.Serialize(new { status = "Success" });
		}
	}
}