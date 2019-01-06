using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HNetPortal.Areas.MVC.Controllers {
	public class VersionController : Controller {

		// GET: MVC/Version
		public ActionResult Index() {
			return View();
		}

		// GET: MVC/Version/Details/5
		//public ActionResult Details(int id) {
		//	return View();
		//}

		//WSH
		// GET: MVC/Version/Details/
		public ActionResult Details() {
			return View();
		}


		// GET: MVC/Version/Create
		public ActionResult Create() {
			return View();
		}

		//// POST: MVC/Version/Create
		//[HttpPost]
		//public ActionResult Create(FormCollection collection) {
		//	try {
		//		// TODO: Add insert logic here

		//		return RedirectToAction("Index");
		//	} catch {
		//		return View();
		//	}
		//}

		//// GET: MVC/Version/Edit/5
		//public ActionResult Edit(int id) {
		//	return View();
		//}

		//// POST: MVC/Version/Edit/5
		//[HttpPost]
		//public ActionResult Edit(int id, FormCollection collection) {
		//	try {
		//		// TODO: Add update logic here

		//		return RedirectToAction("Index");
		//	} catch {
		//		return View();
		//	}
		//}

		//// GET: MVC/Version/Delete/5
		//public ActionResult Delete(int id) {
		//	return View();
		//}

		//// POST: MVC/Version/Delete/5
		//[HttpPost]
		//public ActionResult Delete(int id, FormCollection collection) {
		//	try {
		//		// TODO: Add delete logic here

		//		return RedirectToAction("Index");
		//	} catch {
		//		return View();
		//	}
		//}
	}
}
