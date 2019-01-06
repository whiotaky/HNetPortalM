using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Net.Http.Headers;
using WSHLib;

namespace HNetPortal.Areas.api.Controllers {

	[RoutePrefix("api/TFS")]
	public class TFSController : ApiController {
		
		[HNetAuthorize]
		[HttpGet]
		[Route("BuildResult/{projectName}/{buildName}")]		
		public HttpResponseMessage GetBuildResult(string projectName, string buildName) {

			Logger.Log($"GetBuildResult: {projectName} / {buildName}");
			
			TFBuildResultItem bri = TeamFoundation.GetBuildStatus(projectName, buildName);
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			if (bri.exception != null) {
				Logger.Log("Sending 500 error since TeamFoundation.GetBuildStatus returned an exception");
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);				
			} 	else {
				httpResponseMessage.Content = new ObjectContent<TFBuildResultItem>(bri, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			}			

			return httpResponseMessage;
			
		}
		[HNetAuthorize]
		[HttpPost]
		[Route("ListFiles")]
		public HttpResponseMessage PostFileList([FromBody]Models.GenericRequest req) {

			Logger.Log($"PostListFiles: {req.sourcePathName}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				List<string> fileList_s = TeamFoundation.fileList(req.sourcePathName, new List<string> { "/packages/", "/scripts/scripts/", "/bin/", "/obj/", "/images/", "/fonts/" });
				List<TFSourceItem> outList = new List<TFSourceItem>();
				foreach (var s in fileList_s) {
					TFSourceItem fi = new TFSourceItem(s);
					outList.Add(fi);
				}				
				Logger.Log("Returning list of source files");
				httpResponseMessage.Content = new ObjectContent<List<TFSourceItem>>(outList, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Exception in api TFS controller", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}
			
			return httpResponseMessage;
		}

		[HNetAuthorize(Roles = "Administrators")]
		[HttpPost]
		[Route("FileContent")]
		public HttpResponseMessage PostFileContent([FromBody]Models.GenericRequest req) {

			Logger.Log($"PostFileContent: {req.sourceFileName}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				string sourceContents = TeamFoundation.getSource(req.sourceFileName);
				sourceContents = HttpUtility.HtmlEncode(sourceContents); //7.9.2017: added this to handle code w/ html in it (.aspx pages, etc.)
				TFSourceItem si = new TFSourceItem(req.sourceFileName, sourceContents);
			
				Logger.Log("Returning source file contents");			
				httpResponseMessage.Content = new ObjectContent<TFSourceItem>(si, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			} catch (Exception ex) {
				Logger.LogException("Exception in api TFS controller", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}

	}
}
