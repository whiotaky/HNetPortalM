using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using WSHLib;

namespace HNetPortal.Private {
    public class myImage {
        public string fileName { get; set; }
        public string thumbName { get; set; }
        public int idx { get; set; }
        public string CaroClass { get; set; }
        public string alt { get; set; }
    }

    public partial class ImageDirEdit : System.Web.UI.Page {
		private List<myImage> imageList;
		private string imgBaseDir = "/imagebase/";
        public string whichDir = "";
        public int nextCaroIdx = 0;
        public string isPublic = "N";

        protected void Page_Load(object sender, EventArgs e) {
            string cls;
            string html;

            if (!IsPostBack) {

                whichDir = Request.QueryString["dir"];
                Logger.Log("dir param=" + whichDir);

                imageList = new List<myImage>();
                loadImageList(whichDir);

                DirectoryInfo dir = new DirectoryInfo(Page.Server.MapPath("~/full/" + whichDir));
                this.fpNameLabel.Text = "Full Path: " + whichDir;
                dirNameInput.Text = dir.Name; //gets last dir in path

                List<string> publicDirs = ImageLib.loadPublicDirList();

                if (publicDirs.Contains(whichDir)) {
                    cls = "btn btn-xs btn-success";
                    html = "Make Private&nbsp;<span class=\"glyphicon glyphicon-eye-close\"></span>";
                    isPublic = "Y";
                } else {
                    cls = "btn btn-xs btn-danger";
                    html = "Make Public&nbsp;<span class=\"glyphicon glyphicon-eye-open\"></span>";
                    isPublic = "N";
                }
                secToggleBtn.Attributes["class"] = cls;
                secToggleBtn.InnerHtml = html;

            }

        }


        private void loadImageList(string whichDir) {

            Logger.Log("called for dir " + whichDir);

            //Build list of all files
            string filePath = Page.Server.MapPath(imgBaseDir + "full/" + whichDir);
            DirectoryInfo Dir = new DirectoryInfo(filePath);
            FileInfo[] fList = Dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            myImage myImageItem = null;

            string caroClass = "active";
            foreach (FileInfo FI in fList) {
				myImageItem = new myImage {
					fileName = imgBaseDir + "full/" + whichDir + "/" + FI.Name,
					thumbName = imgBaseDir + "thumb/" + whichDir + "/" + FI.Name,
					idx = nextCaroIdx++,
					CaroClass = caroClass,
					alt = "" //placeholder for possible future use             
				};
				imageList.Add(myImageItem);
                caroClass = "";

                string thumbFullPath = Page.Server.MapPath(imgBaseDir + "thumb/" + whichDir + "/" + FI.Name);
                if (!System.IO.File.Exists(thumbFullPath)) {
                    Logger.Log("creating thumb for " + thumbFullPath);
                    Directory.CreateDirectory(Page.Server.MapPath(imgBaseDir + "thumb/" + whichDir));
                    ImageLib.ResizeImage(filePath + "/" + FI.Name, thumbFullPath, 60, 60, System.Drawing.Imaging.ImageFormat.Png);
                }

            }

            Repeater1.DataSource = imageList;
            Repeater1.DataBind();

        }


        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e) {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
                myImage mi = (myImage)e.Item.DataItem;
                Image img = (Image)e.Item.FindControl("imgThumb");
                img.Attributes["src"] = ResolveUrl("~" + mi.thumbName);
                img.Attributes["alt"] = mi.alt;
                img.Attributes["data-imagePath"] = ResolveUrl("~" + mi.fileName);
            }

        }

    }
}