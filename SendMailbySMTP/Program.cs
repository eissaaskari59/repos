using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SendMailbySMTP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string result = SendEmail("from@site.com", "to@site.com", "subject", "Hi body", "html", null, "", "mailserver");
        }
       
        public static string SendEmail(string from, string To, string subject, string Body, string Messagetype, string[] AttachedFiles, string options = "",string MailServer="")
        {
              string MailServerSetting = "";   string MailServerUsername = "";   string MailServerPassword = "";  string MailServerDefaultSender = "";
                
                if (MailServer.IndexOf(",") >= 0)
                {
                    try { MailServerUsername = MailServer.Split(',')[1]; } catch { }
                    try { MailServerPassword = MailServer.Split(',')[2]; } catch { }
                    try { MailServerDefaultSender = MailServer.Split(',')[3]; } catch { }
                    try { MailServerSetting = MailServer.Split(',')[4]; } catch { }
                    MailServer = MailServer.Split(',')[0];
                }
            
            string res = "";
            bool shouldsend = true;
            try
            {
                if (Messagetype == "") Messagetype = "html";
                from = ValidateMailAddresses(from);
                To = ValidateMailAddresses(To);
                if (To.Length > 0)
                {

                    using (SmtpClient client = new SmtpClient(MailServer))
                    {
                        if (MailServerUsername.Length > 0)
                        {
                            if (MailServer.Split(',').Length>5) client.Port = int.Parse(MailServer.Split(',')[5]);
                            var basicCredential = new NetworkCredential(MailServerUsername, MailServerPassword);
                            client.UseDefaultCredentials = false;
                            client.Credentials = basicCredential;
                            if (MailServerSetting.ToUpper().IndexOf("SSL") >= 0) client.EnableSsl = true;
                            if (MailServerDefaultSender.Length > 0) from = MailServerDefaultSender;
                        }
                        MailAddress From = new MailAddress(from); MailAddress ToM = new MailAddress(from); MailMessage msg = new MailMessage(From, ToM); msg.Subject = subject; msg.Body = Body; if (Messagetype == "html") { msg.IsBodyHtml = true; }

                        msg.To.Clear(); foreach (string item in To.Split(',')) { try { msg.To.Add(new MailAddress(item.Trim(), "")); } catch { res = res + ",Issue in mail address:" + item; } }
                        if (AttachedFiles != null) { foreach (string filename in AttachedFiles) { try { if (File.Exists(filename) == true) { msg.Attachments.Add(new System.Net.Mail.Attachment(filename)); }  } catch { } } }

                        if (options.ToUpper().IndexOf("SENDIFATTACHMENT") >= 0 && msg.Attachments.Count == 0) shouldsend = false;
                        if (shouldsend) { client.Send(msg); msg.Dispose(); res = "ok" + res; } else { res = "Fail:Send Rejected by Options"; }
                      

                        return res;
                    }
                }
            }
            catch { }
            return "Not Sent";
        }
        private static string ValidateMailAddresses(string sTo)
        {
            sTo = sTo.Replace(" ", ",").Replace(Environment.NewLine, ",").Replace(";", ",");
            sTo = sTo.Replace(";", ","); sTo = string.Join(",", sTo.Split(',').ToList().Where(x => x.Length > 0 && x.IndexOf("@") >= 0));
            return sTo;
        }
    }
}
