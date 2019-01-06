using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Web.UI.HtmlControls;
using WSHLib;


//https://weblog.west-wind.com/posts/2009/Nov/07/ClientIDMode-in-ASPNET-40

namespace HNetPortal.Private {

    public class EditListItem {
        public int feedid { get; set; }
        public string feedName { get; set; }
        public byte feedType { get; set; }
        public string enableForUser { get; set; }
        public int orderBy { get; set; }
    }

    public partial class UserRSSEdit : System.Web.UI.Page {
		private List<EditListItem> list1 = new List<EditListItem>();
		private List<EditListItem> list2 = new List<EditListItem>();

        protected void Page_Load(object sender, EventArgs e) {

            //Repeater2.ItemTemplate = Repeater1.ItemTemplate;

            if (!Page.IsPostBack) {
                list1 = loadListFM(1);
                loadListPrefs(ref list1);
                Repeater1.DataSource = list1;
                Repeater1.DataBind();

                list2 = loadListFM(2);
                loadListPrefs(ref list2);
                Repeater2.DataSource = list2;
                Repeater2.DataBind();
            }

        }


        private List<EditListItem> loadListFM(int type) {

            List<EditListItem> list = new List<EditListItem>();
            MySqlConnection conn = new MySqlConnection();
            try {

				conn = new MySqlConnection {
					ConnectionString = Global.getConnString(SupportedDBTypes.MySql)
				};
				conn.Open();
                MySqlCommand cmd = new MySqlCommand("select m.feedid, m.feedname, m.feedType from feedsmaster m where enabled='Y' and feedType=@feedType", conn);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@feedType", type);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
					EditListItem item = new EditListItem {
						feedid = (int)reader[0],
						feedName = (string)reader[1],
						feedType = (byte)reader[2],
						enableForUser = "N",
						orderBy = 0
					};
					list.Add(item);
                }

            } catch (MySqlException ex) {
                Logger.LogException("UserRSSEdit.loadListFM Exception: ", ex);

            } finally {
                conn.Close();
            }

            return list;

        }

		private void loadListPrefs(ref List<EditListItem> list) {

            if (!User.Identity.IsAuthenticated) {
                Logger.Log("IsAuthenticated failed");
                //return "{\"status\": \"Error: Authentication\"}";

                //throw exception here??
                return;
            }

            MySqlConnection conn = new MySqlConnection();
            try {

				conn = new MySqlConnection {
					ConnectionString = Global.getConnString(SupportedDBTypes.MySql)
				};
				conn.Open();

                for (int i = 0; i < list.Count(); i++) {
                    EditListItem item = list[i];
                    MySqlCommand cmd = new MySqlCommand("select orderby from userfeedprefs where username=@username and feedid=@feedid", conn);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@feedid", item.feedid);
                    cmd.Parameters.AddWithValue("@username", User.Identity.Name);
                    MySqlDataReader reader;
                    reader = cmd.ExecuteReader();
                    if (reader.Read()) {
                        item.enableForUser = "Y";
                        item.orderBy = (byte)reader[0];
                        list[i] = item;
                    }
                    reader.Close();
                }

            } catch (MySqlException ex) {
                Logger.LogException("UserRSSEdit.loadListPref Exception: ", ex);

            } finally {
                conn.Close();
            }

        }


        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e) {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {

                HtmlInputControl n_feedid = (HtmlInputControl)e.Item.FindControl("n_feedid");
                CheckBox n_en = (CheckBox)e.Item.FindControl("n_en");
                Label n_fn = (Label)e.Item.FindControl("n_fn");
                TextBox n_ob = (TextBox)e.Item.FindControl("n_ob");

                EditListItem item = (EditListItem)e.Item.DataItem;

                n_en.Checked = item.enableForUser == "Y";
                n_feedid.Value = item.feedid.ToString();
                n_fn.Text = item.feedName;
                n_ob.Text = item.orderBy.ToString();
                n_ob.Enabled = n_en.Checked;

                //n_en.InputAttributes.Add("data-feedid", item.feedid.ToString());

            }

        }

        protected void Repeater2_ItemDataBound(object sender, RepeaterItemEventArgs e) {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {

                HtmlInputControl c_feedid = (HtmlInputControl)e.Item.FindControl("c_feedid");
                CheckBox c_en = (CheckBox)e.Item.FindControl("c_en");
                Label c_fn = (Label)e.Item.FindControl("c_fn");
                TextBox c_ob = (TextBox)e.Item.FindControl("c_ob");

                EditListItem item = (EditListItem)e.Item.DataItem;

                c_en.Checked = item.enableForUser == "Y";
                c_feedid.Value = item.feedid.ToString();
                c_fn.Text = item.feedName;
                c_ob.Text = item.orderBy.ToString();
                c_ob.Enabled = c_en.Checked;


                //c_en.InputAttributes.Add("data-feedid", item.feedid.ToString());

            }

        }

        protected void Button1_Click(object sender, EventArgs e) {

            if (!User.Identity.IsAuthenticated) {
                Logger.Log("IsAuthenticated failed");
                return;
            }

            List<EditListItem> newList = new List<EditListItem>();

            foreach (RepeaterItem ri in Repeater1.Items) {
                HtmlInputControl n_feedid = (HtmlInputControl)ri.FindControl("n_feedid");
                CheckBox n_en = (CheckBox)ri.FindControl("n_en");
                Label n_fn = (Label)ri.FindControl("n_fn");
                TextBox n_ob = (TextBox)ri.FindControl("n_ob");
                
                n_ob.Enabled = n_en.Checked;

                if (n_en != null && n_en.Checked) {
					EditListItem item = new EditListItem {
						feedid = int.Parse(n_feedid.Value),
						feedName = "",
						feedType = 0,
						enableForUser = "Y"
					};
					try {
                        item.orderBy = int.Parse(n_ob.Text);
                    } catch {
                        item.orderBy = 0;
                    }
                    newList.Add(item);
                }

            }

            foreach (RepeaterItem ri in Repeater2.Items) {
                HtmlInputControl c_feedid = (HtmlInputControl)ri.FindControl("c_feedid");
                CheckBox c_en = (CheckBox)ri.FindControl("c_en");
                Label c_fn = (Label)ri.FindControl("c_fn");
                TextBox c_ob = (TextBox)ri.FindControl("c_ob");

                c_ob.Enabled = c_en.Checked;

                if (c_en != null && c_en.Checked) {
					EditListItem item = new EditListItem {
						feedid = int.Parse(c_feedid.Value),
						feedName = "",
						feedType = 0,
						enableForUser = "Y"
					};
					try {
                        item.orderBy = int.Parse(c_ob.Text);
                    } catch {
                        item.orderBy = 0;
                    }
                    newList.Add(item);
                }


                MySqlConnection conn = new MySqlConnection();
                try {

                    conn.ConnectionString = Global.getConnString(SupportedDBTypes.MySql);
                    conn.Open();

                    //delete all prefs for the user first
                    MySqlCommand cmd = new MySqlCommand("delete from userfeedprefs where username=@username", conn);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@username", User.Identity.Name);
                    cmd.ExecuteNonQuery();
                    Logger.Log("userfeedprefs purged for user");

                    //now create prefs records for the user
                    for (int i = 0; i < newList.Count(); i++) {

                        //get a unique upfid 
                        long curr_ufpid = 1;
                        cmd = new MySqlCommand("select max(ufpid)+1 as uppid from userfeedprefs", conn);
                        cmd.Prepare();
                        MySqlDataReader reader;
                        reader = cmd.ExecuteReader();
                        if (reader.Read()) {
                            curr_ufpid = (long)reader[0];
                        }
                        reader.Close();
                        Logger.Log("curr_ufpid is " + curr_ufpid.ToString());

                        //create the pref rec now
                        EditListItem item = newList[i];
                        cmd = new MySqlCommand("insert into userfeedprefs values(@ufpid,@feedid,@username,@orderby)", conn);
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@ufpid", curr_ufpid);
                        cmd.Parameters.AddWithValue("@feedid", item.feedid);
                        cmd.Parameters.AddWithValue("@username", User.Identity.Name);
                        cmd.Parameters.AddWithValue("@orderby", item.orderBy);
                        cmd.ExecuteNonQuery();
                        Logger.Log("userfeedprefs created for " + curr_ufpid.ToString());

                    }

                    if (Page.IsPostBack) {
                        //string script = "bootbox.alert(\"Your edits were successfully saved.\");";
                        string script2 = @"bootbox.alert({
                                        message: 'Your edits were successfully saved.',
                                        size: 'small'
                                        });  ";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "savedMsgScript", script2, true);
                    }

                } catch (Exception ex) {
                    Logger.LogException("Button1_Click for user " + User.Identity.Name, ex);

                } finally {
                    conn.Close();
                }

               
            }

        }

    }
}