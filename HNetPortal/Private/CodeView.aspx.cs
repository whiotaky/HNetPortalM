using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// For a someday maybe...search feature
// https://stackoverflow.com/questions/41039/find-in-files-search-all-code-in-team-foundation-server

namespace HNetPortal.Private {
    public partial class CodeView : Page {

        protected void Page_Load(object sender, EventArgs e) {

            foreach (var folderName in TeamFoundation.FolderList(@"$/WSH/DotNet")) {
                projectsLB.Items.Add(folderName);
            }

        }

    }

}