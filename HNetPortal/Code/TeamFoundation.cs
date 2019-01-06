using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Configuration;
using WSHLib;


/*
 * Build service WSH / Archive all is currently running on an agent on Web01.  The agent runs as web1px.  Connecting to the
 * tfs server via the api's used in this module, we use VssAadCredential().  Called without credentials, the account running the 
 * site will be used for authentication (also web1px).  Finally, web1px is a "member" of the tfs project WSH.
 */

namespace HNetPortal {

    public class TFBuildResultItem {
        public string projectName { get; set; }
        public string buildName { get; set; }
        public int id { get; set; }
        public string status { get; set; }
        public string result { get; set; }
        public DateTime started { get; set; }
        public DateTime finished { get; set; }
        public string log { get; set; }
        public Exception exception { get; set; }
    }

    public class TFSourceItem {
        public string itemName { get; set; }
        public string sourceContents { get; set; }
        public TFSourceItem (string name) {
            itemName = name;
            sourceContents = "";
        }
        public TFSourceItem(string name, string contents) {
            itemName = name;
            sourceContents = contents;
        }
    }


    public class TeamFoundation {

        public static TFBuildResultItem GetBuildStatus(string projectName, string buildName) {

			TFBuildResultItem ret = new TFBuildResultItem {
				projectName = projectName,
				exception = null
			};

			try {

                string tfsUrl = (string)ConfigurationManager.AppSettings["TFS_URL"];
                var buildClient = new BuildHttpClient(new Uri(tfsUrl), new VssAadCredential()); //does windows AD Auth
                var builds = buildClient.GetBuildsAsync(projectName);
				//throw (new Exception("Test Exception"));
                //This assumes the build results are returned in latest to oldest order, so
                //the first result that matches the build name for the selected project is
                //presumed to be the most recent.
                Build b = null;
                foreach (var build in builds.Result) {
                    if (build.Definition.Name.ToLower().Equals(buildName.ToLower()) ) {
                        b = (Build)build;
                        Logger.Log("Found the latest build results for " + projectName + " " + buildName);
                        break;
                    }
                }

                if (b == null)
                    throw new Exception("No build results found for buildname=" + buildName);

				if (b.StartTime != null) {
					ret.started = TimeZone.CurrentTimeZone.ToLocalTime((DateTime)b.StartTime);
				} else {
					Logger.Log("startTime is null");
				}                
                if (b.FinishTime != null) {
                    ret.finished = TimeZone.CurrentTimeZone.ToLocalTime((DateTime)b.FinishTime);
                } else {
                    Logger.Log("FinishTime is null status is " + b.Status);
                }

                ret.buildName = b.Definition.Name;
                ret.id = b.Id;
                ret.status = b.Status.ToString();
                ret.result = b.Result.ToString();
                System.Text.StringBuilder s = new System.Text.StringBuilder();

                //get the log "file" for the build
                var logs = buildClient.GetBuildLogsAsync(projectName, b.Id);
                foreach (var log in logs.Result) {
                    var logLines = buildClient.GetBuildLogLinesAsync(projectName, b.Id, log.Id);
                    foreach (var line in logLines.Result) {
                        s.AppendLine((string)line);
                    }
                }

                ret.log = s.ToString();
                Logger.Log("Seems to have worked");

            } catch (Exception ex) {
				ret = new TFBuildResultItem {
					exception = ex,
					projectName = projectName
				};

				Logger.LogException("TeamFoundation.GetBuildStatus: exception", ex);
            }

            Logger.Log("finished");
            return ret;

        }


        private static List<string> GetItemList(string startAt, ItemType listType, List<string> filePathFilters = null) {

            List<string> ret = new List<string>();

            Logger.Log(string.Format("ListType: \"{1}\" Filters: \"{0}\"", filePathFilters==null ? "NULL": String.Join("|", filePathFilters.ToArray() ),
                       Enum.GetName(typeof(ItemType),listType)  ));

            try {
                string tfsUrl = (string)ConfigurationManager.AppSettings["TFS_URL"];
                TfsTeamProjectCollection server = new TfsTeamProjectCollection(new Uri(tfsUrl));
                VersionControlServer version = server.GetService(typeof(VersionControlServer)) as VersionControlServer;
                Logger.Log("TFS connection appears to be good");

                //ItemSet items = version.GetItems(@"$\WSH\DotNet", RecursionType.OneLevel);            
                //ItemSet items = version.GetItems(@"$\ProjectName\FileName.cs", RecursionType.Full);
                ItemSet items = version.GetItems(startAt, VersionSpec.Latest, listType == ItemType.Folder ? RecursionType.OneLevel : RecursionType.Full);
                Logger.Log(string.Format("Got {0} items from TFS server", items.Items.Count()));

                foreach (Item item in items.Items) {

                    //apply filter(s)
                    if (filePathFilters != null) {
                        if (filePathFilters.Any(str => item.ServerItem.Contains(str))) {
                            continue;
                        }
                    }

                    //for whatever reason, project lists will contain the parent folder.  Skip it.
                    string itemName = (string) item.ServerItem;
                    if (startAt.ToLower().Equals(itemName.ToLower())) {                       
                        continue;
                    }

                    //ensure the item's type matches the requested type
                    if (listType == ItemType.File) {
                        if (item.ItemType.Equals(listType))
                            ret.Add(item.ServerItem);
                    } else {
                        ret.Add(item.ServerItem);
                    }

                }

            } catch (Exception ex) {
                Logger.LogException("tfs GetItems:", ex);
            }

            Logger.Log(string.Format("Finished.  Returning {0} items from TFS server", ret.Count()));
            return ret;

        }


        public static List<string> FolderList(string startAt, List<string> filePathFilters = null) {
            Logger.Log("called");
            return GetItemList(startAt, ItemType.Folder, filePathFilters);

        }


        public static List<string> fileList(string startAt, List<string> filePathFilters = null) {
            Logger.Log("called");
            return GetItemList(startAt, ItemType.File, filePathFilters);

        }


        public static string getSource(string sourceFileName) {

            string ret = "";

            try {
                string tfsUrl = (string)ConfigurationManager.AppSettings["TFS_URL"];
                TfsTeamProjectCollection server = new TfsTeamProjectCollection(new Uri(tfsUrl));
                VersionControlServer versionControl = server.GetService(typeof(VersionControlServer)) as VersionControlServer;
                Logger.Log("TFS connection appears to be good");

                // Listen for the Source Control events.
                //versionControl.NonFatalError += OnNonFatalError;
                // versionControl.Getting += OnGetting;
                // versionControl.BeforeCheckinPendingChange += OnBeforeCheckinPendingChange;
                // versionControl.NewPendingChange += OnNewPendingChange;

                var item = versionControl.GetItem(sourceFileName, VersionSpec.Latest);
                var encoding = System.Text.Encoding.GetEncoding(item.Encoding);

                using (var stream = item.DownloadFile()) {
                    int size = (int)item.ContentLength;
                    var bytes = new byte[size];
                    stream.Read(bytes, 0, size);
                    ret = encoding.GetString(bytes);
                }

            } catch (Exception ex) {
                Logger.LogException("tfs GetItems:", ex);
            }

            Logger.Log("finished");
            return ret;
        }

    }
}