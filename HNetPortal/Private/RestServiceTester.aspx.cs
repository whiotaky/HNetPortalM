using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using WSHLib;

namespace HNetPortal.Private {
    public partial class RestServiceTester : System.Web.UI.Page {

        public string serviceBaseUrl = "";
        public string token = "";
        public string APIKey = "NOKEY";
        private static object locker = new object();

        protected void Page_Load(object sender, EventArgs e) {

            serviceBaseUrl = (string)ConfigurationManager.AppSettings["ServiceBaseUrl"];

            var str = "restdevpx:xxxxxxxxxxxx212";
           
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(str);
            var s = System.Convert.ToBase64String(plainTextBytes);
            token =s;

            APIKey = getAPIKeyFromSystem();

            string s2 = Global.getConnString(SupportedDBTypes.MySql);
            


        }

        private string getAPIKeyFromSystem() {

            string ret = "";
            int hours = 1;

            string apiFile = (string)ConfigurationManager.AppSettings["APIKeyFile"];
            Logger.Log("apiFile=" + apiFile);

            DateTime threshold = DateTime.Now.AddHours(-hours);
            DateTime fd = File.GetLastWriteTime(apiFile);
            bool tooOld = (fd < threshold);
            Logger.Log(string.Format("{0} : {1}", threshold, fd));

            if (tooOld) {
                Logger.Log("Key needs cycling");
                try {
                    lock (locker) {
                        string newKey = Guid.NewGuid().ToString();
                        Logger.Log("Generated New Key=" + newKey);
                        File.WriteAllText(apiFile, newKey);
                        ret = newKey;
                    }
                } catch(Exception ex) {
                    Logger.LogException("RestServiceTester: ",ex);
                }

            } else {
                ret = File.ReadLines(apiFile).First();
                Logger.Log("Key is new enough returning key from file " + ret);
            }

            return ret;
        }

    }
}