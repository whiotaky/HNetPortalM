using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WSHLib;


/*/benfoster.io/blog/aspnet-mvc-custom-error-pages
 * This page is enabled by this in web.config<system.web>
 * FOR TESTING ONLOCAL MACHINE: mode= "On".Just to verify a general error (such as setting dbpassword to incorrect)
 *  <customErrors mode ="RemoteOnly" defaultRedirect="~/ErrorPages/GeneralError.aspx"  redirectMode="ResponseRewrite">
        <error statusCode="404" redirect="~/ErrorPages/404.aspx" />
      </customErrors>

  this also in system.webserver for your 404 page
   <httpErrors errorMode="Custom" defaultResponseMode="File">
      <remove statusCode="404" subStatusCode="-1" />
      <error statusCode="404" path="/ErrorPages/404.aspx" responseMode="ExecuteURL" />
    </httpErrors>
 */



namespace HNetPortal.ErrorPages {
    public partial class _404 : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {

           
            string referer = Request.QueryString["aspxerrorpath"] ?? Request.RawUrl;
            Server.ClearError();
            Response.Status = "404 not found";
            Response.StatusCode = 404;

            Logger.Log("Page_Load: 404.aspx, referer="+referer);

        }
    }

}