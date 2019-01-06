using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ical.Net;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.IO;
using WSHLib;

namespace HNetPortal.Private {
    public partial class ICalImport : System.Web.UI.Page {
		private List<ICal.ICalItem> list = new List<ICal.ICalItem>();
		private bool inclEnd = false;
		private bool inclSummary = false;
		private bool inclDescr = false;
		private bool inclLocation = false;
		private string whereToPut = "top";
		private string iCalFileName = "";
       
        protected void Page_Load(object sender, EventArgs e) {

            Label1.Visible = false;

            if (Page.IsPostBack) {
               

                if (!User.Identity.IsAuthenticated) {
                    Logger.Log("IsAuthenticated failed, skipping all postback code");
                    return;
                }
                

                //Gather all params into variables.  The params we want
                //are all prefixed with IMPORT_.  The selected iCal items to
                //import are all stuffed into list.
                //
                foreach (String key in Request.Params.AllKeys) {

                    //if (key.StartsWith("IMPORT_")) {
                    //    Logger.Log("ICalImport.PageLoad: key=" + key + " value: " + Request.Params[key]);
                    //}

                    if (key.StartsWith("IMPORT_UID_")) {
                        string value = key; //the param 'value' is 'on', so we really want the key substring :)
                        value = value.Substring(11);
                        Logger.Log("UID param parsed value=" + value);
						ICal.ICalItem item = new ICal.ICalItem {
							uid = value
						};
						list.Add(item);
                    }

                    if (key.Equals("IMPORT_inclEnd"))
                        inclEnd = true;

                    if (key.Equals("IMPORT_inclSummary"))
                        inclSummary = true;

                    if (key.Equals("IMPORT_inclDescr"))
                        inclDescr = true;

                    if (key.Equals("IMPORT_inclLocation"))
                        inclLocation = true;

                    if (key.Equals("IMPORT_whereToPut"))
                        whereToPut = Request.Params[key];

                    if (key.Equals("IMPORT_iCalFileName"))
                        iCalFileName = Request.Params[key];

                }
                Logger.Log(string.Format("param vars: inclEnd={0}, inclSummary={1}, inclDescr={2},inclLocation={3}, whereToPut={4}, iCalFileName={5}", inclEnd, inclSummary, inclDescr, inclLocation, whereToPut, iCalFileName));

                string msg = "Import "+iCalFileName+" completed";

                //Read the iCal import file and update each list record with it's 
                //iCal details found in the file.  Items not already in the list
                //were de-selected in the UI, and are hence ignored.
                //
                string workDir = ConfigurationManager.AppSettings["WorkDir"];
                string workFileName = workDir + "/" + User.Identity.Name + "_" + iCalFileName;
                try {

					Ical.Net.Calendar calendar = null;
					using (StreamReader sr = new StreamReader(workFileName)) {
						calendar = Ical.Net.Calendar.Load(sr.ReadToEnd());
					}

					for (int i = 0; i < calendar.Events.Count(); i++) {

						Ical.Net.CalendarComponents.CalendarEvent ev = calendar.Events[i];

						ICal.ICalItem item = new ICal.ICalItem {
							uid = ev.Uid,
							summary = ev.Summary,
							startDate = String.Format("{0:yyyy-MM-dd hh:mm tt}", ev.DtStart.AsSystemLocal),
							endDate = String.Format("{0:yyyy-MM-dd hh:mm tt}", ev.DtEnd != null ? ev.DtEnd.AsSystemLocal : ev.DtStart.AsSystemLocal),
							location = ev.Location,
							description = ev.Description
						};
						ICal.ICalItem foundItem = list.Find(x => x.uid.Contains(item.uid));                      
                        if (foundItem != null) {
                            int idx = list.IndexOf(foundItem);
                            list[idx] = item;
                        } else {
                            Logger.Log($"ICalImport.PageLoad: NOT FOUND ITEM {item.uid}");
                        }
                    }

                } catch (Exception ex) {
                    Logger.LogException($"ICalImport.PageLoad iCal read loop error: ", ex);
                }

                //Now loop through the list and do the sql updates to the user's calendar.
                //
                MySqlConnection conn = new MySqlConnection();
                try {
                    conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
                    conn.Open();
                    
                    for (int i = 0; i < list.Count(); i++) {
						ICal.ICalItem ev = list[i];
                        string calDate = ev.startDate.Substring(0, 10);
                        Logger.Log("db op for event on " + calDate);
                        string content = "";
                        string rowContent = "";
                        string sql = "insert into calendar set username=@username, caldate=@calDate, content=@content";

                        //fetch content for calDate, if any
                        MySqlCommand cmd = new MySqlCommand("select content from calendar where username=@username and calDate=@caldate", conn);
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@username", User.Identity.Name);
                        cmd.Parameters.AddWithValue("@caldate", calDate);
                        MySqlDataReader reader;
                        reader = cmd.ExecuteReader();
                        if (reader.Read()) {
                            rowContent = (string)reader[0];
                            sql = "replace into calendar set username=@username, caldate=@calDate, content=@content";
                            Logger.Log("sql will UPDATE event on " + calDate);
                        }
                        reader.Close();

                        //append event details to content, depending on
                        //selected params
                        content = string.Format("{0}{1}", content, ev.startDate.Substring(11));
                        if (inclEnd)
                            content = string.Format("{0} TO {1}", content, ev.endDate.Substring(11));
                        if(inclSummary)
                            content = string.Format("{0}, {1}", content, ev.summary);
                        if(inclDescr)
                            content = string.Format("{0}, {1}", content, ev.description);
                        if(inclLocation)
                            content = string.Format("{0}, {1}", content, ev.location);

                        if(whereToPut.Equals("top") && !rowContent.Equals(""))
                            content = string.Format("{0}\r\n{1}", content, rowContent);

                        if (whereToPut.Equals("bottom") && !rowContent.Equals(""))
                            content = string.Format("{1}\r\n{0}", content, rowContent);

                        //perform to insert/replace statement now
                        cmd = new MySqlCommand(sql, conn);
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@content", content);
                        cmd.Parameters.AddWithValue("@calDate", calDate);
                        cmd.Parameters.AddWithValue("@username", User.Identity.Name);
                        cmd.ExecuteNonQuery();

                        Logger.Log("db op SUCCEEDED for event on " + calDate);
                    }

                } catch (Exception ex) {
                    Logger.LogException("ICalImport.PageLoad SQL Loop: ", ex);
                   msg="Error Happened, see log";
                } finally {
                    conn.Close();
                }
                Logger.Log("Finished");
                Label1.Text = msg;
                Label1.Visible = true;
            }
         
        }

    }
}