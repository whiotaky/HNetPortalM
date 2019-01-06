using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeePassLib.Serialization;
using KeePassLib.Keys;
using HNetPortal;
using KeePassLib;
using KeePassLib.Delegates;
using KeePassLib.Collections;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security;
using WSHLib;

namespace HNetPortal {

	public class KeePass {
		public class VaultEntryItem {
			public string uuid { get; set; }
			public string UserName { get; set; }
			public byte[] PasswordEncrypted { get;  set; }
			public string PasswordEncryptedBase64 { get; set; }
			public string Title { get; set; }
			public string URL { get; set; }
			public string Notes { get; set; }
		}

		private KeePassLib.PwDatabase theDB;
		public KeePass(string dbPath, SecureString dbPassword) {

			try {
				IOConnectionInfo ioConnInfo = new IOConnectionInfo { Path = dbPath };
				CompositeKey compKey = new CompositeKey();
				compKey.AddUserKey(new KcpPassword(Crypto.UnSecureString(dbPassword)));
				theDB = new KeePassLib.PwDatabase();
				theDB.Open(ioConnInfo, compKey, null);
				Logger.Log($"Opened successfully {dbPath}");
			} catch (Exception ex) {
				theDB = null;
				Logger.LogException("KeePass contructor failed: ", ex);
			}

		}

		~KeePass() {
			Logger.Log($"destructor executing");
			if (theDB != null) {
				Logger.Log($"Closing the db");
				theDB.Close();
			}
		}

		/// <summary>
		/// Tell whether the KeePass database is open
		/// </summary>
		public bool isOpen() {
			if (theDB != null) {
				return theDB.IsOpen;
			}
			return false;
		}

		/// <summary>
		/// Returns a masked a string
		/// </summary>
		public static string MaskString(string s, char maskChar = '*') {
			return s = new String(s.Select(r => maskChar).ToArray());
		}

		/// <summary>
		/// Returns a list of KeePass entries that match the requested title
		/// </summary>
		public PwObjectList<PwEntry> FindByTitle(string targetTitle) {

			if (isOpen()) {
				SearchParameters sp = new SearchParameters();
				sp.SearchInUuids = false;
				sp.SearchInUserNames = false;
				sp.SearchInUrls = false;
				sp.SearchInTags = false;
				sp.SearchInStringNames = false;
				sp.SearchInGroupNames = false;
				sp.SearchInNotes = false;
				sp.SearchInPasswords = false;
				sp.SearchInTitles = true;
				sp.SearchString = targetTitle;
				PwObjectList<PwEntry> results = new PwObjectList<PwEntry>();
				theDB.RootGroup.SearchEntries(sp, results);				
				return results;
			} else {
				return null;
			}
		}

		/// <summary>
		/// Dumps the DB to a string.  NEVER use this in production environment due to unsecure password string usage!
		/// </summary>
		public string Dump(bool maskPwd = true) {

			string ret = "";

			if (isOpen()) {

				ret = ret + "<h1>Get Groups</h1>";
				foreach (KeePassLib.PwGroup g in theDB.RootGroup.GetGroups(true)) {
					ret += g.Name + "<BR>";
				}

				ret = ret + "<h1>Get Entries nosub</h1>";
				foreach (KeePassLib.PwEntry kwp in theDB.RootGroup.GetEntries(false)) {
					string groupName = kwp.ParentGroup.Name;
					string title = kwp.Strings.ReadSafe("Title");
					string url = kwp.Strings.ReadSafe("URL");
					string un = kwp.Strings.ReadSafe("UserName");
					string pw = kwp.Strings.ReadSafe("Password");
					string notes = kwp.Strings.ReadSafe("Notes");
					string noWeb = kwp.Strings.ReadSafe("NOWEB");

					if (!noWeb.Equals(string.Empty))
						continue;

					if (maskPwd)
						pw = MaskString(pw);

					ret = ret + $"<b>G:{groupName}</b>, Title:{title}, url:{url},UserName: {un}, Password:{pw} NoWeb:{noWeb}, notes:{notes}<hr><br/>\n";
				}

				ret = ret + "<h1>Get Entries</h1>";
				foreach (KeePassLib.PwEntry kwp in theDB.RootGroup.GetEntries(true)) {
					string groupName = kwp.ParentGroup.Name;
					string title = kwp.Strings.ReadSafe("Title");
					string url = kwp.Strings.ReadSafe("URL");
					string un = kwp.Strings.ReadSafe("UserName");
					string pw = kwp.Strings.ReadSafe("Password");
					string hnet = kwp.Strings.ReadSafe("HNETENABLED");
					string notes = kwp.Strings.ReadSafe("Notes");

					if (maskPwd)
						pw = MaskString(pw);

					ret = ret + $"<b>G:{groupName}</b>, Title:{title}, url:{url},UserName: {un}, Password:{pw} HNET:{hnet}, notes:{notes}<hr><br/>\n";
				}
			} else {
				ret = "database is not open";
			}
			return ret;
		}

		/// <summary>
		/// Returns the main (first) directory node
		/// </summary>
		public KeePassLib.PwGroup getRoot() {
			return theDB?.RootGroup;
		}

		/// <summary>
		/// Returns a valid Javascript object for insertion into a page, of the requested tree node
		/// </summary>
		public string TraverseJS(KeePassLib.PwGroup theGroup) {

			//NOTES:  This is a recursive function.  It really sucks how I did it and should be re-written if
			//you ever have issues with it because it's overly complex. Should probably build a linked list first then 
			//build the JS off that.  Rewrite using newtonsoft.
			//It assumes the home element is named "root".

			StringBuilder ret = new StringBuilder();
			if (theGroup == null) {
				Logger.Log($"TraverseJS received null group, returning empty string");
				return ret.ToString();
			}
		
			Logger.Log($"TraverseJS Start for {theGroup.Name}");
			
			string groupSettings = @"
						icon: 'glyphicon glyphicon-stop',
						selectedIcon: 'glyphicon glyphicon-stop',
						color: '#051c67',
						backColor: '#FFFFFF',
						href: '#node-1',
						selectable: true,
						state:
								{
									checked: true,
									disabled: false,
									expanded: {0},
									selected: false
									},";
			groupSettings = groupSettings.Replace("{0}", theGroup.Name.ToUpper().Equals("ROOT") ? "true" : "false");
			int groupCount = theGroup.GetGroups(false).Where(x => !x.Name.ToUpper().Equals("RECYCLE BIN")).Count();
			int entryCount = theGroup.GetEntries(false).Where(x => x.Strings.ReadSafe("NOWEB").Equals(string.Empty)).Count();

			if (isOpen()) {

				ret.Append($"\t\t\t{{ text: \"{theGroup.Name}\",\n{groupSettings}\n");

				string delim = "";
				Logger.Log($"{theGroup.Name} has {entryCount} entries ");

				if (theGroup.GetEntries(false).Count() > 0) {

					int totCount = groupCount + entryCount;
					ret.Append("tags: ['"+ totCount + "'], nodes: [\n");
					foreach (KeePassLib.PwEntry kwp in theGroup.GetEntries(false)
								.Where(x => x.Strings.ReadSafe("NOWEB").Equals(string.Empty))
								.OrderBy(x => x.Strings.ReadSafe("Title").ToLower())) {

						string groupName = kwp.ParentGroup.Name;
						string title = kwp.Strings.ReadSafe("Title");					
						string url = kwp.Strings.ReadSafe("URL");
						string un = kwp.Strings.ReadSafe("UserName");
						string notes = kwp.Strings.ReadSafe("Notes");
						string uuid = kwp.Uuid.ToHexString();
					
						title = title.Replace("'", "\\'");
						notes = notes.Replace("'", "\\'");
						un = un.Replace("'", "\\'");
						url = url.Replace("'", "\\'");

						ret.Append(string.Format("{1}\t\t\t{{text: '{2}',uuid:'{0}' }}", uuid, delim, title));
						delim = ",\n";
					}
					
					if (groupCount == 0) {
						ret.Append($"\n]// End nodes for {theGroup.Name} \n");
					}
					
				} else {
					if (groupCount == 0) {
						ret.Append("tags: [0] \n");
					}  else {
						int totCount = groupCount + entryCount;
						ret.Append("tags: ['" + totCount + "'], nodes: [\n");
					}
				}

				if (groupCount > 0 ) {

					if (entryCount > 0) {
						ret.Append(", //has more entries\n");
					}
				
					Logger.Log($"{theGroup.Name} has {groupCount} children to traverse ");
					foreach (KeePassLib.PwGroup childGroup in theGroup.GetGroups(false)
								.Where(x => !x.Name.ToUpper().Equals("RECYCLE BIN"))
								.OrderBy(x=> x.Name.ToLower())) {

						ret.Append(TraverseJS(childGroup));
					}
					ret.Append("]},\n");
				} else {
					Logger.Log($"{theGroup.Name} has NO children to traverse ");
					ret.Append($"}}, // No children for {theGroup.Name} \n");
				}

			}

			Logger.Log($"TraverseJS End for {theGroup.Name}");
			return ret.ToString();
		}

		/// <summary>
		/// Returns a single keepass entry based on the requested uuid
		/// </summary>
		public PwEntry FindByUUID(string uuid) {

			Logger.Log($"Doing find uuid for {uuid}");
			if (isOpen()) {

				SearchParameters sp = new SearchParameters {
					SearchInUuids = true,
					SearchInUserNames = false,
					SearchInUrls = false,
					SearchInTags = false,
					SearchInStringNames = false,
					SearchInGroupNames = false,
					SearchInNotes = false,
					SearchInPasswords = false,
					SearchInTitles = false,
					SearchString = uuid
				};
							
				PwObjectList<PwEntry> results = new PwObjectList<PwEntry>();
				theDB.RootGroup.SearchEntries(sp, results);
				Logger.Log($"search entries completed; found {results.Count()}");
				return results.FirstOrDefault();

			} else {
				Logger.Log("db wasn't open so returning null");
				return null;
			}
		}
	
		public VaultEntryItem EncryptEntry(PwEntry entry) {

			Logger.Log($"Start");
			if (entry == null) {
				Logger.Log($"Cant do anything with a null entry");
				return null;
			}

			try {
				KeePass.VaultEntryItem ret = new KeePass.VaultEntryItem {
					uuid = entry.Uuid.ToHexString(), 
					Title = (string)entry.Strings.ReadSafe("Title"),
					Notes = (string)entry.Strings.ReadSafe("Notes"),
					UserName = (string)entry.Strings.ReadSafe("UserName"),
					URL = (string)entry.Strings.ReadSafe("URL")
				};

				//several varieties of passwords, all encrypted or masked.
				string encryptionKey = Global.PortalSettings.KeePassEncrptionKey;
				byte[] bs = Crypto.EncryptStringToBytes(entry.Strings.ReadSafe("Password"), Encoding.UTF8.GetBytes(encryptionKey), Encoding.UTF8.GetBytes(encryptionKey));
				ret.PasswordEncrypted = bs;
				ret.PasswordEncryptedBase64 = Convert.ToBase64String(bs);
				Logger.Log($"Worked");
				return ret;

			} catch (Exception ex) {
				Logger.LogException("Exception: ", ex);
				return null;
			}
						
		}
	}

}
