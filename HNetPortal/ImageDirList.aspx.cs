using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using WSHLib;

namespace HNetPortal {

    public class myDirInfo {
        public string dirName { get; set; }
        public string parentDir { get; set; }
        public bool hasSubdirs { get; set; }
        public bool hasFiles { get; set; }
    }

    public partial class ImageDirList : System.Web.UI.Page {

        public List<string> publicDirs;
        protected void Page_Load(object sender, EventArgs e) {

            string whichDir = "/imagebase/";

            if (!IsPostBack) {

                publicDirs = ImageLib.loadPublicDirList();
              
                ContentPlaceHolder c2 = (ContentPlaceHolder)Page.Master.FindControl("ContentPlaceHolder");
                LoginView lv = (c2.FindControl("LoginView1") as LoginView);

                if (User.Identity.IsAuthenticated) {

                    DirectoryInfo dir = new DirectoryInfo(Page.Server.MapPath("~" + whichDir + "/full/"));//give path of Root directory
					TreeNode tnRoot = new TreeNode {
						Text = "<b>Index</b>",
						NavigateUrl = "#"
					};

					TreeView TreeView1 = (TreeView)lv.FindControl("TreeView1");
                    TreeView1.Nodes.Add(tnRoot);

                    Logger.Log("User is authenticated, creating ADD link");
					TreeNode newLink = new TreeNode {
						SelectAction = TreeNodeSelectAction.None,
						Text = "<button id='newDirBtn' class='btn btn-xs btn-success' style='margin-bottom: 3px;'>Add New Dir</button>"
					};
					tnRoot.ChildNodes.Add(newLink);
                    AddDirs(dir, tnRoot, "");
                    TreeView1.CollapseAll();
                    tnRoot.Expand();
                } else {  //Guest/Public Access
                    ListView ListView1 = (ListView)lv.FindControl("ListView1");
                    ListView1.DataSource = publicDirs;
                    ListView1.DataBind();
                }
               
            }

        }


        public void AddDirs(DirectoryInfo dir, TreeNode parent, string webParentDir) {

            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (DirectoryInfo d in dirs) {

                //this is th individual, directory-level security check work
                //by default all directories are private.  They must be marked as 
                //public in order for them to be viewed without a login
                string secString = webParentDir + "/" + d.Name;
                Logger.Log("secString=" + secString);

				TreeNode tn = new TreeNode {
					SelectAction = TreeNodeSelectAction.None
				};
				string pat = "/ImageDirDisplay.aspx?dir=" + webParentDir + "/" + d.Name;

                if (User.Identity.IsAuthenticated) {
                    string glyphIcon = "";
                    string editLink = "";

                    Logger.Log("User IsAuthenticated, creating EDIT link");
                    editLink = "<a href='/Private/ImageDirEdit.aspx?dir=" + webParentDir + "/" + d.Name + "' class='btn btn-xs btn-primary' style='margin-bottom: 3px;'>Edit</a>" +
                        "&nbsp;|&nbsp;<button class='btn btn-xs btn-danger' data-dirName='" + webParentDir + "/" + d.Name + "'style='margin-bottom: 3px;' onClick='deleteDirClicked(this);return false;'>Delete</button>&nbsp;&nbsp;";

                    if (publicDirs.Contains(secString)) {
                        Logger.Log("SecString Matches, adding glyphIcon for admin");
                        glyphIcon = "&nbsp;&nbsp;<span class=\"glyphicon glyphicon-eye-open\" style='color: green'></span>";
                    }

                    tn.Text = editLink + "<a href='" + pat + "'>" + d.Name + "</a>" + glyphIcon;
                    parent.ChildNodes.Add(tn);

                } else {

                    if (publicDirs.Contains(secString)) {
                        tn.Text = "<a href='" + pat + "'>" + d.Name + "</a>";
                        Logger.Log("Public user AND secString permitted for " + d.Name);
                        parent.ChildNodes.Add(tn);
                    } else {                      
                        tn.Text = "<span style='color: lightgray' title='Not authorized!'>" + d.Name + "</span>";
                        parent.ChildNodes.Add(tn);
                    }

                }

                AddDirs(d, tn, webParentDir + "/" + d.Name);
            }

        }

    }
}
