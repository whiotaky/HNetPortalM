using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Script.Serialization;
using WSHLib;
using WSHLib.Network;

namespace HNetPortal.Areas.api.Controllers {

	//Apply Authorize to the entire class.
	//(Means only logged in Hnet Users are allowed to call any method.)
	//Alternatively, remove it from here and place it selectively above 
	//individual method(s) that you want to restrict.
	//[HNetAuthorize(Roles = "Administrators")]
	[HNetAuthorize]
	public class NodeStatusController : ApiController {

		// GET: api/NodeStatus	
		public List<NetNodeItem> Get() {			
			Logger.Log($"GET api/NodeStatus  user={User.Identity.Name}");
			return (Get(0));
		}

		// GET: api/NodeStatus/0 or 1
		public List<NetNodeItem> Get(int id) {
			Logger.Log($"GET api/NodeStatus/n  user={User.Identity.Name}");

			var jsonSerialiser = new JavaScriptSerializer();
			bool allowCached = (id == 0);

			if (allowCached) {
				string fromCache = Cache.Get("netnodes", 30);
				if (!fromCache.Contains("CACHE GET ERROR") &&
				!fromCache.Contains("CACHE NOT FOUND")) {
					Logger.Log("got from cache so ending");
					return  JsonConvert.DeserializeObject<List<NetNodeItem>>(fromCache);					
				}
			} else {
				Logger.Log("allowCached FALSE, so getting fresh for" + User.Identity.Name);
			}

			NetNodes n = new NetNodes();
			Cache.Put("netnodes", jsonSerialiser.Serialize(n.GetNodeList() ));
			Logger.Log("End");
			return n.GetNodeList();
		}

		// POST: api/NodeStatus		
		public List<NetNodeItem> Post([FromBody]Models.GenericRequest req) {
			Logger.Log($"POST api/NodeStatus allowCached={req.allowCached}  user={User.Identity.Name}");
			return Get(req.allowCached ? 0 : 1);				
		}
	
	}
}
