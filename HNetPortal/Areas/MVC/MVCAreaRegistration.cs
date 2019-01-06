using System.Web.Mvc;

namespace HNetPortal.Areas.MVC {
	public class MVCAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "MVC";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			context.MapRoute(
				"Default",
				"MVC/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}