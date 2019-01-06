using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using WSHLib;

// http://jsfiddle.net/ivanarvizu/JS6JV/16/

namespace HNetPortal {

    public class myImage {
        public string fileName { get; set; }
        public string thumbName { get; set; }
        public int idx { get; set; }
        public string CaroClass { get; set; }
        public string alt { get; set; }
    }

    public partial class ImageDirDisplay : System.Web.UI.Page {
		private List<myImage> imageList;
		private string imgBaseDir = "/imagebase/";

        protected void Page_Load(object sender, EventArgs e) {

            string whichDir = Request.QueryString["dir"];
            Logger.Log("dir param=" + whichDir);

            List<string> publicDirs = ImageLib.loadPublicDirList();
            if (!publicDirs.Contains(whichDir) && !User.Identity.IsAuthenticated) {            
                Logger.Log("Private Image Directory Display requested for anonymous user!  Redirecting public default page");
                Response.Redirect("/");
            }

            string crumbs = "<a href='/ImageDirList.aspx'>Index</a> | ";
            string[] dirs = whichDir.Split('/');
            string pat = "";

            if (dirs.Count() > 0) {
                for (int i = 1; i < dirs.Count(); i++) {
                    pat += dirs[i];
                    crumbs += linkNode(pat, dirs[i]) + " / ";
                    pat += "/";
                }
            }
            dirLabel.InnerHtml = crumbs.Remove(crumbs.Length - 2, 2); //strip off trailing slash

            if (!IsPostBack) {
                imageList = new List<myImage>();
                loadImageList(whichDir);
            }

        }


        private string linkNode(string dirPath, string dirName) {

            string ret = "";
            DirectoryInfo dir = new DirectoryInfo(Page.Server.MapPath(imgBaseDir + "full/" + dirPath));
            DirectoryInfo[] dirs = dir.Parent.GetDirectories();
            string pdir = dir.Parent.Name + "/";
            if (pdir.Equals("full/")) {
                pdir = "";
            }

            ret = "<div class=\"btn-group\"><button type=\"button\" class=\"btn btn-xs btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">" +
                dirName +
                "<span class=\"caret\"></span></button><ul class=\"dropdown-menu\">";

            foreach (DirectoryInfo d in dirs) {
                string link = " <a href='?dir=/" + pdir + d.Name + "'>" + d.Name + "</a>";
                ret += "<li>" + link + "</li>";
            }
            ret += "</ul></div>\n";
            return ret;

        }


        private void loadImageList(string whichDir) {

            Logger.Log("Called for dir " + whichDir);

            //Build list of all files
            string filePath = Page.Server.MapPath(imgBaseDir + "full/" + whichDir);
            DirectoryInfo Dir = new DirectoryInfo(filePath);
            FileInfo[] fList = Dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            myImage myImageItem = null;
            int idx = 0;
            string caroClass = "active";
            foreach (FileInfo FI in fList) {
				myImageItem = new myImage {
					fileName = imgBaseDir + "full/" + whichDir + "/" + FI.Name,
					thumbName = imgBaseDir + "thumb/" + whichDir + "/" + FI.Name,
					idx = idx++,
					CaroClass = caroClass,
					alt = "" //placeholder for possible future use             
				};
				imageList.Add(myImageItem);
                caroClass = "";

                string thumbFullPath = Page.Server.MapPath(imgBaseDir + "thumb/" + whichDir + "/" + FI.Name);
                if (!System.IO.File.Exists(thumbFullPath)) {
                    Logger.Log("Creating thumb for " + thumbFullPath);
                    Directory.CreateDirectory(Page.Server.MapPath(imgBaseDir + "thumb/" + whichDir));
                    ImageLib.ResizeImage(filePath + "/" + FI.Name, thumbFullPath, 60, 60, System.Drawing.Imaging.ImageFormat.Png);
                }

            }

            Repeater1.DataSource = imageList;
            Repeater2.DataSource = imageList;
            Repeater1.DataBind();
            Repeater2.DataBind();

        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e) {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
                myImage mi = (myImage)e.Item.DataItem;
                Image img = (Image)e.Item.FindControl("imgThumb");
                img.Attributes["src"] = ResolveUrl("~" + mi.thumbName);
                img.Attributes["alt"] = mi.alt;
            }

        }

        protected void Repeater2_ItemDataBound(object sender, RepeaterItemEventArgs e) {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {

                myImage mi = (myImage)e.Item.DataItem;
                System.Web.UI.HtmlControls.HtmlGenericControl imgDiv = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("imgDiv");
                Image img = (Image)e.Item.FindControl("imgFull");

                string divClass = "item";
                if (!mi.CaroClass.Equals("")) {
                    divClass = "item active";
                    img.Attributes["src"] = ResolveUrl("~" + mi.fileName);
                } else {
                    img.Attributes["data-src"] = ResolveUrl("~" + mi.fileName);
                }

                imgDiv.Attributes["class"] = divClass;
                img.Attributes["alt"] = mi.alt;

            }

        }

    }
}
