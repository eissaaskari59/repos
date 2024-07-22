using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SendFilesByFTP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string result = SendFilesByFTPNormal(@"c:\temp\1.txt", "user", "pass", "ftp.host.com", "", "", "*.*", "/dir/");
        }
        public static bool FtpFileExist(string ftpUsername, string ftpPassword, string ftpaddress)
        {
            var request = (FtpWebRequest)WebRequest.Create(ftpaddress);
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            request.Method = WebRequestMethods.Ftp.GetFileSize;


            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {

            }
            return false;
        }
        public static string SendFilesByFTPNormal(string source, string userName, string Password, string hostName, string Port, string options, string FileNameFormat, string dir)
        {
            List<string> items = new List<string>();
            int count = 0;
            string result = "";
            try
            {
                if (FileNameFormat == "") FileNameFormat = "*.*";

                if (File.Exists(source) == true)
                {
                    items.Add(source);
                }
                else
                {
                    items = Directory.GetFiles(source, FileNameFormat).ToList();
                }
                if (dir.Length > 0) hostName = hostName + dir;
                {
                    result = "";
                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(userName, Password);
                        result = result + "Connected Successfully(FTP),  ";
                        if (items.Count == 0) result = result + "|No Files Found!";
                        foreach (string item in items)
                        {
                            result = result + "|";
                            try
                            {
                                result = result + "|" + Path.GetFileName(item) + ":";
                                if (File.Exists(item) == true)
                                {
                                    string ftppath = "ftp://" + hostName + "/" + Path.GetFileName(item);

                                    if (FtpFileExist(userName, Password, ftppath) == true)
                                    {
                                        result = result + ":Already Exist!";
                                    }
                                    else
                                    {
                                        byte[] rawResponse = client.UploadFile(ftppath, WebRequestMethods.Ftp.UploadFile, item);
                                        result = result + "Sent by FTP ,|";
                                        count = count + 1;
                                    }
                                }
                                else
                                {
                                    result = result + "Gone";
                                }
                            }
                            catch (System.Exception ex)
                            {
                                result = result + "ERR0R:(" + ex.Message + ")";
                            }
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                string err = ex.ToString();
                result = result + "ERR0R67:" + ex.Message.Replace("|", "").Replace("-", "").Replace(";", "");
            }
            count = items.Count;
            result = "FTP Send Result ,Server:" + hostName + " ,User:" + userName + " ,Port:" + Port + ",) " + (count > 0 ? count + " ..Files.. Found! " : "No File Found! ") + result;
            return result;
        }


    }
}
