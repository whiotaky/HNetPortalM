using System.Web.Mvc;

namespace HNetPortal.Areas.MVC.Controllers {
	public class DefaultController : Controller {
		// GET: MVC/Default
		public ActionResult Index() {
			HNetPortal.Areas.MVC.Models.Default d = new Models.Default {
				DemoText = "Hello world!",
				DemoMessage = "Hello World, my model contains this message."
			};
			
			var testParam =  this.Request.Params["test"];
			if (testParam!=null) {
				d.DemoText = (string)testParam;
			}
			
			return this.RazorView(d);
			//return this.RazorView();
		}

	}
}