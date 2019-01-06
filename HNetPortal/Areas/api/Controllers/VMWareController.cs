using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WSHLib;
using WSHLib.Network;

namespace HNetPortal.Areas.api.Controllers {

	[HNetAuthorize(Roles = "Administrators")]
	[RoutePrefix("api/VMWare")]
	public class VMWareController : ApiController {
	
		[Route("GetState/{vmName}")]
		[HttpGet]
		public HttpResponseMessage GetState(string vmName) {

			Logger.Log($"GetVMState for {vmName}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				string localFileName = (string)ConfigurationManager.AppSettings["WorkDir"] + "/esxiBackup.log";
				string privateKeyFileName = HttpContext.Current.Server.MapPath(@"~/App_Data") + "/lrpx_esxi";
				VMWare vmware = new VMWare(localFileName, privateKeyFileName);
				if (vmware.Connect() != 0) {
					Logger.Log("Couldn't connect to ESXI server");
					httpResponseMessage = Request.CreateResponse(HttpStatusCode.NoContent);
				} else {
					VMState vmstate = vmware.GetVMStateByName(vmName);
					Logger.Log("Returning VMState as " + vmstate.ToString());
					httpResponseMessage.Content = new ObjectContent<string>(vmstate.ToString(), Configuration.Formatters.JsonFormatter);
					httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
				}

			} catch (Exception ex) {
				Logger.LogException("GetState Exception in api VMWare Controller", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}
			
			return httpResponseMessage;
		}


		[Route("SetState/{vmName}/{setToState}")]
		[HttpGet]
		public HttpResponseMessage SetState(string vmName, string setToState) {

			Logger.Log($"SetVMState for {vmName} setToState={setToState}");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				string localFileName = (string)ConfigurationManager.AppSettings["WorkDir"] + "/esxiBackup.log";
				string privateKeyFileName = HttpContext.Current.Server.MapPath(@"~/App_Data") + "/lrpx_esxi";
				VMWare vmware = new VMWare(localFileName, privateKeyFileName);

				if (vmware.Connect() != 0) {
					Logger.Log("Couldn't connect to ESXI server");
					httpResponseMessage = Request.CreateResponse(HttpStatusCode.NoContent);
				} else {

					VMState res = VMState.Unknown;
					if (setToState == "PowerOn") {
						Logger.Log("Sending STARTUP request for " + vmName);
						res = vmware.VMStartup(vmName);
					} else if (setToState == "Shutdown") {
						Logger.Log("Sending SHUTDOWN request for " + vmName);
						res = vmware.VMShutdown(vmName);
					}

					httpResponseMessage.Content = new ObjectContent<string>(res.ToString(), Configuration.Formatters.JsonFormatter);
					httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
				}

			} catch (Exception ex) {
				Logger.LogException("SetState Exception in api VMWare Controller", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			return httpResponseMessage;
		}

	}

}
