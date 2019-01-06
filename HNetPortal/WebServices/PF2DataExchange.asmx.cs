using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using PFTrack2;
using WSHLib;

namespace HNetPortal.WebServices {

	/// <summary>
	/// SOAP API for the PFTrack win32 client
	/// </summary>
	[WebService(Namespace = "https://portalm.hiotaky.com/WebServices/", Description = "Exchanges data with pfTrack Win32 application")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class PF2DataExchange : System.Web.Services.WebService {

		[WebMethod(Description ="PFTrack2 Data Exchange version number")]
		public string Version() {
			Logger.Log("Called");
			return Soap.Version;
		}

		[WebMethod(Description = "Provides all available details for each symbol requested.")]
		public DataSet GetQuotesDetailed(string symbols, string licenseKey) {
			Logger.Log("Called");
			PFTrack2.Quote q = new PFTrack2.Quote();
			return q.GetQuotesDetailed(symbols, licenseKey);
		}

		[WebMethod(Description = "Provides basic information for each symbol requested.")]
		public DataSet GetQuotes(string symbols) {
			Logger.Log($"Called for symbols: {symbols}");
			PFTrack2.Quote q = new PFTrack2.Quote();
			return q.GetQuotesDS(symbols);
		}

		[WebMethod(Description = "Returns all PFTrack2 tables.")]
		public DataSet ExportData(string userName, string password) {
			Logger.Log($"Called for userName {userName}");
			return Soap.ExportData(userName, password);
		}

		[WebMethod(Description = "Replaces all PFTrack2 data with data you provide in xmlDoc.")]
		public string ImportData(string xmlDoc, string userName, string password, string version) {
			Logger.Log($"Called for userName {userName}");
			return Soap.ImportData(xmlDoc, userName, password, version, false);
		}

		[WebMethod(Description = "Replaces all PFTrack2 data with data you provide in xmlDoc.")]
		public string ImportDataChunk(string xmlDoc, string userName, string password, string version) {
			Logger.Log($"Called for userName {userName}");
			return Soap.ImportData(xmlDoc, userName, password, version, true);
		}

		[WebMethod(Description = "Empties all tables for the provided user name.")]
		public string PurgeTables(string userName, string password, string version) {
			Logger.Log($"Called for userName {userName}");
			return Soap.PurgeTables(userName, password, version);
		}

	}
}
