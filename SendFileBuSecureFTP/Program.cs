using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendFileBuSecureFTP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string result = SendFilesByFTPS("c:\temp\file.txt", "user", "pass", "host", "22", "*.*", "/dir/");
        }
        public static string SendFilesByFTPS(string source, string ftpUsername, string ftpPassword, string ftpaddress, string port, string FileNameFormat, string dir)
        {
            string result = "";
            List<string> items = new List<string>();
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
                if (items.Count == 0) { result = result + "|No Files Found!"; return result; }
                    result = "";
                    

                    var client = new SftpClient(ftpaddress, ftpUsername, ftpPassword);
                    if (port.Length > 0)
                    {
                        client = new SftpClient(ftpaddress, int.Parse(port), ftpUsername, ftpPassword);
                    }
                    client.Connect();
                    result = result + "Connected Successfully(SFTP),  ";
                    try { client.ChangeDirectory(dir); } catch { }

                    foreach (string item in items)
                    {
                        result = result + "|";
                        try
                        {
                            result = result + "|" + Path.GetFileName(item) + ":";
                            if (File.Exists(item) == true)
                            {
                                using (var fileStream = new FileStream(item, FileMode.Open))
                                {
                                    client.BufferSize = 4 * 1024;
                                    client.UploadFile(fileStream, Path.GetFileName(item));
                                    result = result + "Sent By FTPS";
                                }
                            }
                            else
                            {
                                result = result + "Gone";
                            }
                        }
                        catch (System.Exception ex)
                        {
                            result = result + "ERR0R70:(" + ex.Message + ")";
                        }
                    client.Disconnect();
                    client.Dispose();
                }


            }
            catch (System.Exception ex)
            {
                string err = ex.ToString();
                result = result + "ERR0R66:" + ex.Message.Replace("|", "").Replace("-", "").Replace(";", "");
            }
            int count = items.Count;
            result = "FTPS Send Result ,Server:" + ftpaddress + " ,User:" + ftpUsername + " ,Port:" + port + ",) " + (count > 0 ? count + " ..Files.. Found! " : "No File Found! ") + result;
            return result;
        }
    }
}
