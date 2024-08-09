using Danx.Imap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ReadMailsByIMAP
{
    internal class Program
    {
        static void Main(string[] args)
        {

            List<KeyValuePair<string, string>> allresult = new List<KeyValuePair<string, string>>();
            string result = MailCommandsHandleByMail(ref allresult, "server;port;user;pass;", 0);

        }
        public static List<KeyValuePair<uint, string>> SearchMailBoxItems(ImapClient Client, uint LastID, string MailFrom, string MailProfileKey, string mailbox = null)
        {
            List<KeyValuePair<uint, string>> result = new List<KeyValuePair<uint, string>>();
          
            try
            {

                IEnumerable<uint> uids = Enumerable.Empty<uint>();

                if (MailFrom.Length > 0)
                {
                    foreach (string item in MailFrom.Split('|'))
                    {
                        IEnumerable<uint> uids2;
                        uids2 = Client.Search(SearchCondition.From(item), mailbox);
                        uids = uids.Concat(uids2);
                    }
                    uids = uids.Where(x => x > LastID).ToList();
                }
                else
                {
                    IEnumerable<uint> uids2;
                    uids2 = Client.Search(SearchCondition.GreaterThan(LastID), mailbox);
                    uids = uids.Concat(uids2);
                }
                uids = uids.Distinct();
                if (mailbox == null) mailbox = "Inbox";
                foreach (uint id in uids)
                {
                    if (id != LastID) result.Add(new KeyValuePair<uint, string>(id, mailbox));
                }
            }
            catch { }
            return result;
        }
        public static string MailCommandsHandleByMail(ref List<KeyValuePair<string, string>> allresult, string ImapServerUserPass, uint LastID = 0)
        {
            string MailOptions = "";
            string MailProfileKey = "ReadMails_Main_MailCommands";
            string result = "";
            try
            {
                string MailAttachmentNameFormat = ""; string[] MailAttachmentNameFormats = MailAttachmentNameFormat.Split('|');
                string SetName = ""; try { SetName = ImapServerUserPass.Split(',')[5] + ":"; } catch { }
                if (ImapServerUserPass.Length < 3) return "";
                if (MailOptions.IndexOf("SpecialServer:") >= 0) { int st = MailOptions.IndexOf("SpecialServer:") + "SpecialServer:".Length; int en = MailOptions.IndexOf("|", st); ImapServerUserPass = MailOptions.Substring(st, en - st); }
                try
                {
                    string ARchiveFolder = "Imported"; try { ARchiveFolder = ImapServerUserPass.Split(',')[4]; } catch { }
                    using (ImapClient Client = new ImapClient(ImapServerUserPass.Split(',')[0], int.Parse(ImapServerUserPass.Split(',')[1]), true))
                    {
                        try
                        {
                            Client.Login(ImapServerUserPass.Split(',')[2], ImapServerUserPass.Split(',')[3], AuthMethod.Login);
                        }
                        catch (System.Exception ex)
                        {
                            return "Error in username,password for connect to imap mail " + ex.Message;
                        }
                        List<KeyValuePair<uint, string>> uids = SearchMailBoxItems(Client, LastID, "", MailProfileKey);
                        uids.AddRange(SearchMailBoxItems(Client, LastID, "", MailProfileKey, "[Gmail]/Spam"));
                        foreach (KeyValuePair<uint, string> MailItemKeyValuePair in uids)
                        {
                            try
                            {
                                MailMessage item = null;
                                result = result + "<br>,MailID:" + MailItemKeyValuePair.Key + " ";
                                if (MailItemKeyValuePair.Value == "Inbox")
                                {
                                    item = Client.GetMessage(MailItemKeyValuePair.Key);
                                }
                                else
                                {
                                    item = Client.GetMessage(MailItemKeyValuePair.Key, true, MailItemKeyValuePair.Value);
                                }
                                string thisfrom = item.From.Address; 
                                
                                result = result + item.Subject + "," + thisfrom + ", ";

                                
                            }
                            catch (System.Exception ex) { string err = ex.ToString(); result = err; }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    result = result + "Error in connect to imap mail server" + ex.Message;
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                string err = ex.ToString();
                result = result + ex.Message;
                
            }
            return result;
        }
    }
}
