using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSHLib;
using PFTrack2;
using System.Net.Http.Headers;

namespace HNetPortal.Areas.api.Controllers {


	[HNetAuthorize]
	[RoutePrefix("api/PFTrack2")]
	public class PFTrack2Controller : ApiController {

		// GET: api/PFTrack/Account/Legacy
		[Route("Account/Legacy")]
		[HttpGet]
		public HttpResponseMessage AccountLegacy() {

			Logger.Log($"AccountLegacy");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
			//tester();
			try {

				Account a = new Account(User.Identity.Name);
				List<AccountItemLegacy> ret = (List<AccountItemLegacy>)a.GetList(Account.Format.Legacy, DateTime.Now.ToString());
				ret = ret.Where(x => x.value > 0).ToList();
				httpResponseMessage.Content = new ObjectContent<List<AccountItemLegacy>>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
				
			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			//System.Threading.Thread.Sleep(2000);
			return httpResponseMessage;
		}


		// Get: api/PFTrack/Portfolio/Legacy/{accountNo}
		[Route("Portfolio/Legacy/{accountNo}")]
		[HttpGet]
		public HttpResponseMessage PortfolioLegacy(string accountNo) {

			Logger.Log($"PortfolioLegacy");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {				
				Portfolio p = new Portfolio(User.Identity.Name, accountNo);
				List <PortfolioItemLegacy> ret = (List<PortfolioItemLegacy>)p.GetList(Portfolio.Format.Legacy,null,Portfolio.HIDEZEROBAL);
				httpResponseMessage.Content = new ObjectContent<List<PortfolioItemLegacy>>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			//System.Threading.Thread.Sleep(2000);
			return httpResponseMessage;
		}

		// Get: api/PFTrack/Transactions/Legacy/{accountNo}/{symbol}
		[Route("Transactions/Legacy/{accountNo}/{symbol}")]
		[HttpGet]
		public HttpResponseMessage TransactionsLegacy(string accountNo, string symbol) {

			Logger.Log($"TransactionsLegacy");
			HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

			try {
				Transaction t = new Transaction(User.Identity.Name, accountNo, symbol);
				List<TransactionItemLegacy> ret = (List<TransactionItemLegacy>)t.GetList(Portfolio.Format.Legacy, null);
				httpResponseMessage.Content = new ObjectContent<List<TransactionItemLegacy>>(ret, Configuration.Formatters.JsonFormatter);
				httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

			} catch (Exception ex) {
				Logger.LogException("Get Exception", ex);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
			}

			//System.Threading.Thread.Sleep(2000);
			return httpResponseMessage;
		}

		public void tester() {

			//TransType tt = new TransType();
			//var t = tt.GetTransTypes();
			//var ttn = tt.TableName;
			//tt.TableName = "";

			//Transaction tr = new Transaction("boil", "X351", "FCASH");
			//var ts1 = tr.GetList(PFTrack2Base.Format.Legacy, null);
			//var ts2 = tr.GetList(PFTrack2Base.Format.Legacy, DateTime.Now.AddDays(-3650));
			//var tx = ts1;

			//var tsn1 = tr.TableName;

			//var tx = tr.GetList(PFTrack2Base.Format.Normal);
			
			//var y = tx;

			//Account a = new Account("Boil");
			//var testlist1 = a.GetList(Account.Format.Legacy);
			//var testlist2 = a.GetList(Account.Format.Legacy, DateTime.Now.AddDays(-3650));
			//var testlist3 = a.GetList(Account.Format.Normal);		
			//var testlist5 = a.GetList();


			//PFTrack2.Portfolio p = new Portfolio(User.Identity.Name, "X351");
			//List<PortfolioItemLegacy> foo = (List<PortfolioItemLegacy>)p.GetList(Portfolio.Format.Legacy, null, Portfolio.HIDEZEROBAL);
			//var test2 = p.GetList();

		}

	}


}
