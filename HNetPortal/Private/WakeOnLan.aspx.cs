using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Net.Sockets;
using System.Data;
using WSHLib;

namespace HNetPortal.Private {
    public partial class WakeOnLan : System.Web.UI.Page    {

        protected void Page_Load(object sender, EventArgs e)    {

            Logger.Log("begin");
            if (Page.IsPostBack) {
                string whichMAC = whichToWake.Value;
                string whichAName = this.whichAName.Value;

                Logger.Log("whichToWake=" + this.whichToWake.Value);
                try {
                    // 9C:5C:8E:7A:87:84 
                    //byte[] mac = new byte[] { 0x00, 0x0F, 0x1F, 0x20, 0x2D, 0x35 };
                    byte[] mac = whichMAC.Split(':').Select(x => Convert.ToByte(x, 16)).ToArray(); 
					WSHLib.Network.WakeOnLan.WakeUp(mac);
                    //throw new Exception();
                    Logger.Log("Sent Packet to "+whichAName);
                    successMsg.InnerHtml = "<span class='glyphicon glyphicon-ok-sign'></span>&nbsp;Magic Packet successfully sent to "+whichAName+" at MAC address " + whichMAC;
                    successMsg.Style.Value = "display:normal";
                    errorMsg.Style.Value = "display:none";
                } catch(Exception ex) {
                    Logger.LogException("Error creating mac bytes array", ex);
                    errorMsg.InnerHtml = "<span class='glyphicon glyphicon-warning-sign'></span>&nbsp;Error creating mac bytes array to " + whichAName + " at MAC address " + whichMAC;
                    errorMsg.Style.Value = "display:normal";
                    successMsg.Style.Value = "display:none";
                }

            }

        }
        
    }
}