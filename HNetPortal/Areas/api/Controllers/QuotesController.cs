using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using WSHLib;
using PFTrack2;


namespace HNetPortal.Areas.api {

	[HNetAuthorize]
	public class QuotesController : ApiController {

		// POST api/<controller>	
		public List<QuoteBase> Post([FromBody]Models.GenericRequest req) {

			Logger.Log($"POST api/Quotes  symbols={req.symbols}");

			if (req.allowCached) {
				string fromCache = Cache.Get("quotes", 30);
				
				if (!fromCache.Contains("CACHE GET ERROR") &&
				!fromCache.Contains("CACHE NOT FOUND")) {
					Logger.Log("got from cache, so ending");
					return JsonConvert.DeserializeObject<List<QuoteBase>>(fromCache);
				}
			} else {
				Logger.Log("allowCached FALSE, so getting fresh for" + User.Identity.Name);
			}

			List<QuoteBase> list = Quote.GetQuotesList(req.symbols);
			var jsonSerialiser = new JavaScriptSerializer();
			Cache.Put("quotes", jsonSerialiser.Serialize(list));
			Logger.Log("end");

			return list;
		}	
	}
}