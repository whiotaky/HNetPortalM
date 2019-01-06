using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Configuration;
using WSHLib;
using WSHLib.Network;


namespace HNetPortal.Private {
    public partial class VMTool : System.Web.UI.Page {
		private List<VMItem> vmList = new List<VMItem>();

        protected void Page_Load(object sender, EventArgs e) {
            
            if (!Page.IsPostBack) {

				string localFileName = (string)ConfigurationManager.AppSettings["WorkDir"] + "/esxiBackup.log";
				string privateKeyFileName = HttpContext.Current.Server.MapPath(@"~/App_Data") + "/lrpx_esxi";

				VMWare vmware = new VMWare(localFileName, privateKeyFileName);
                vmware.Connect();
                vmList = vmware.GetVMList();
                vmware.Disconnect();
                Logger.Log(string.Format("Got {0} VMItems from the ESXI server for my datasource", vmList.Count()));
                Repeater1.DataSource = vmList.OrderBy(vm => vm.vmName);
                Repeater1.DataBind();
               
            }

        }
    }
}