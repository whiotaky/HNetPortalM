using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HNetPortal.Private {
    public partial class FeedMasterEdit : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            SqlDataSource1.ConnectionString = Global.getConnString(SupportedDBTypes.MySql) ;
        }
    }
}