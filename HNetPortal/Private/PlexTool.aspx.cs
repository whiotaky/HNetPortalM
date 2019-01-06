using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WSHLib;


namespace HNetPortal.Private {
    public partial class PlexTool : System.Web.UI.Page {
        public string plexSec = "";
        protected void Page_Load(object sender, EventArgs e) {
            plexSec = ConfigurationManager.AppSettings["PLEX_HOMESECTION_NUM"];

            //There may be a more elegant web.config way to do this with roles, but  this works fine for now
            if (!User.IsInRole("Administrators")) {
                Logger.Log("Page_Load: non-Administrator attempted page load.  Throwing general exception now");
                throw new Exception("Non-Administrators are not permitted to load the Plex Tool Page");
            }

        }
    }

}