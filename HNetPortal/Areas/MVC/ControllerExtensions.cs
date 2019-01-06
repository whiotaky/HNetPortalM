using System.Web.Mvc;
using WSHLib;


namespace HNetPortal.Areas.MVC.Controllers {

	//http://www.eworldui.net/blog/post/2011/01/07/Using-Razor-Pages-with-WebForms-Master-Pages.aspx
	public static class ControllerExtensions {

		public static ViewResult RazorView(this Controller controller) {
			return RazorView(controller, null, null);
		}

		public static ViewResult RazorView(this Controller controller, object model) {
			return RazorView(controller, null, model);
		}

		public static ViewResult RazorView(this Controller controller, string viewName) {
			return RazorView(controller, viewName, null);
		}

		public static ViewResult RazorView(this Controller controller, string viewName, object model) {

			string loginViewLogout = (string)controller.Request["__EVENTTARGET"];
			
			//if (loginViewLogout != null && loginViewLogout.Equals("ctl00$LoginView$HeadLoginStatus$ctl00")) {
			  if (loginViewLogout != null && loginViewLogout.Equals("ctl00$ctl04$LoginView$HeadLoginStatus$ctl00")) {
				Logger.Log("Logout detected in MVC Page!  Signing out the user now!");
				Global.SignOutUser();
			}

			if (model != null)
				controller.ViewData.Model = model;

			controller.ViewBag._ViewName = GetViewName(controller, viewName);

			return new ViewResult {
				ViewName = "RazorView",
				ViewData = controller.ViewData,
				TempData = controller.TempData
			};
		}

		private static string GetViewName(Controller controller, string viewName) {
			return !string.IsNullOrEmpty(viewName)	? viewName	: controller.RouteData.GetRequiredString("action");
		}

	}

}