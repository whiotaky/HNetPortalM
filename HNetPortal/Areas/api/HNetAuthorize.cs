using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http;

namespace HNetPortal.Areas.api {
	public class HNetAuthorize : AuthorizeAttribute {
		protected override void HandleUnauthorizedRequest(HttpActionContext actionContext) {

			if (actionContext == null) {
				//throw HttpErrorKeys.ArgumentNull("actionContext");
				//WSH Hell idk wtf to do here
				base.HandleUnauthorizedRequest(actionContext);
			}
		
			//WSH Seems like a hack, but I want a Json response with a lowercase status element since 
			//so many hnetportal ajax calls check for it.
			string whichRole = Roles.Equals(string.Empty) ? "users" : Roles;			
			actionContext.Response = new HttpResponseMessage {
				StatusCode = HttpStatusCode.Unauthorized,
				Content = new StringContent($"{{\"status\": \"Error: Access to this api endpoint is restricted to authenticated HNet {whichRole} only\"}}"),
			};

		}
	}
}