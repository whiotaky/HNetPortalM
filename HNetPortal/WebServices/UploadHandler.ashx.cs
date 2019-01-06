using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.IO;
using WSHLib;

namespace HNetPortal.WebServices {
    /// <summary>
    /// Summary description for UploadHandler
    /// </summary>
    public class UploadHandler : WebService, IHttpHandler {

		//http://www.aspbucket.com/2015/12/file-upload-using-jquery-in-aspnet.html

		private readonly string imgBaseDir = "/imagebase/";

        public void ProcessRequest(HttpContext context) {

            string whichCaller = context.Request["caller"];          
            string filePath = "";
            string thumbFilePath = "";

            if (whichCaller == null) {
                Logger.Log("caller param is null, so we throw error");
                context.Response.Write("{\"status\": \"Error: Null Parameter\"}");
                throw new Exception("caller Param Error: null");

            } else if (whichCaller.ToLower().Equals("ical")) {
                if (!User.Identity.IsAuthenticated) {
                    Logger.Log("IsAuthenticated failed, we throw error now");
                    context.Response.Write("{\"status\": \"Error: Authentication\"}");

                    //evidently this is how you tell the caller things didnt go well
                    throw new Exception("Auth Error: iCal");                
                }
                string workDir = ConfigurationManager.AppSettings["WorkDir"];
                filePath  = workDir + "/" + User.Identity.Name + "_";

            } else if (whichCaller.ToLower().Equals("ib")) {
                if (!User.Identity.IsAuthenticated) {
                    Logger.Log("Non-Admin attempt to upload image");
                    context.Response.Write("{\"status\": \"Error: Authentication\"}");
                    throw new Exception("Auth Error: Non-Admin attempt to upload image");
                }
                string whichDir = context.Request["dir"];
                DirectoryInfo dir = new DirectoryInfo(context.Server.MapPath ("~"+imgBaseDir +"/full/" + whichDir));            
                filePath = dir.FullName + "/";

                DirectoryInfo tdir = new DirectoryInfo(context.Server.MapPath("~" + imgBaseDir + "/thumb/" + whichDir));
                thumbFilePath = tdir.FullName + "/";

            } else {
                Logger.Log("caller param is unknown, so we throw error");
                context.Response.Write("{\"status\": \"Error: Bad Parameter\"}");
                throw new Exception("caller Param Error: unknown param");
            }
            
            if (context.Request.Files.Count > 0) {
                try {
                    HttpFileCollection files = context.Request.Files;
                    for (int i = 0; i < files.Count; i++) {
                        HttpPostedFile file = files[i];
                        string fname = Path.GetFileName(file.FileName);

                        string saveTo = filePath + fname; 
                        file.SaveAs(saveTo);
                        Logger.Log("saved file " + saveTo);

                        //if uploaded an imagebase file, create the thumbnail file now so it's 
                        //immadiately available to the ImageDirEdit page.
                        if (whichCaller.ToLower().Equals("ib")) {
                            string thumbFile = thumbFilePath + fname;
                            if (!System.IO.File.Exists(thumbFile)) {
                                Logger.Log("creating thumb for " + thumbFile);
                                Directory.CreateDirectory(thumbFilePath);
                                ImageLib.ResizeImage(saveTo, thumbFile, 60, 60, System.Drawing.Imaging.ImageFormat.Png);
                            }                           
                        }
                    }

                } catch (Exception ex) {
                    context.Response.Write("{\"status\": \"Error: ex=" + ex.Message + "\"}");
                    //evidently this is how you tell the caller things didnt go well
                    Logger.LogException("UploadHandler.ProcessRequest() Save file: ", ex);
                    throw new Exception("Save file exception: " + ex.Message);
                    //return;
                }

                //System.Threading.Thread.Sleep(4000);

                Logger.Log("finished");
                context.Response.ContentType = "application/json";
                context.Response.Write("{}"); //wsh, no idea about this

            }

        }
        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}