using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GetFilesByFTP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string result = ReadFilesFromFTP("user", "pass", "host", 0, "", "*.*", @"c:\temp\ftpfiles", "/out/");
        }
        public static List<string> FTPGetFiles(string ftpUsername, string ftpPassword, string ftpaddress, string folder, out string ourmessege, string port = "", string options = "", string FileNameFormat = "")
        {
            string[] FileNameFormats = FileNameFormat.Split('|');
            List<string> all = new List<string>();
            try
            {
                if (port.Length > 0) port = ":" + port;
                var request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpaddress + port + folder);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var reader = new StreamReader(responseStream);
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line.Length > 3 && line.Substring(0, 2) == "dr") continue;
                            string[] arr = line.Split(' ');
                            string PartBeforeFilename = "";
                            Regex regexTime = new Regex("[0-9][0-9]:[0-9][0-9]");
                            Regex regexYear = new Regex(@"^(19|20)\d{2}$");
                            foreach (string ite in arr)
                            {

                                if (regexTime.IsMatch(ite) == true)
                                {
                                    PartBeforeFilename = ite;
                                    break;
                                }
                            }

                            if (PartBeforeFilename.Length == 0)
                            {
                                foreach (string ite in arr)
                                {
                                    if (regexYear.IsMatch(ite) == true)
                                    {
                                        PartBeforeFilename = ite;
                                        break;
                                    }
                                }
                            }
                            if (PartBeforeFilename.Length > 0)
                            {
                                if (string.IsNullOrWhiteSpace(line) == false)
                                {
                                    if (line.Length < 50) continue;
                                    int start = line.IndexOf(PartBeforeFilename) + PartBeforeFilename.Length;
                                    string item = line.Substring(start, line.Length - start).Trim();
                                    if (item.Length > 2)
                                    {
                                        int formatmatch = 0;
                                        foreach (string filenameformat in FileNameFormats)
                                        { formatmatch = 1; if (PatternMatch(item, filenameformat) == true) { formatmatch = 2; break; } }

                                        if (formatmatch == 1) continue;
                                        all.Add(item);
                                    }

                                }
                            }
                        }
                    }
                }
                ourmessege = "GetFtpList-ok-" + (all.Count > 0 ? all.Count + " ..Files.. Found!" : "No File Found!");
            }
            catch (System.Exception ex)
            {
                ourmessege = "-ERR0R22-" + ex.Message;
                string outp = ex.ToString();
            }
            return all;
        }
        public static bool PatternMatch(string name, string pattern, string source = "File")
        {
            if (pattern == "*" || name == pattern || pattern == "") return true; if (name.IndexOf(".") < 0 && name.IndexOf("*") < 0) name = name + "*.*";
            if (pattern.IndexOf(".") < 0 && pattern.IndexOf("*") < 0) pattern = pattern + "*.*"; if (pattern.Length > 5 && name.ToUpper().IndexOf(pattern.ToUpper()) >= 0) return true;
            if (pattern.Replace("*.*", "").Length >= 4 && pattern.IndexOf("*") != 0 && name.ToUpper().IndexOf(pattern.Replace("*.*", "").ToUpper()) == 0) return true;
            if (pattern.Replace("*.*", "").Length >= 4 && pattern.IndexOf("*") == 0 && name.ToUpper().IndexOf(pattern.Replace("*.*", "").Replace("*", "").ToUpper()) >= 0) return true;
            if (pattern.Replace("*.*", "").Length >= 4 && source == "Mail" && name.ToUpper().IndexOf(pattern.Replace("*.*", "").ToUpper()) >= 0) return true;
            if (name == "") return false;
            return false;
        }
        public static string FTPGetFile(string ftpUsername, string ftpPassword, string ftpaddress, string folder, string localFilePath, string filename, string port = "", string options = "")
        {
            string res = "";
            try
            {
                
                
                if (Directory.Exists(localFilePath) == false)
                {
                    res = "-Error-Get-" + "Directory Does not Exist!";
                }
                else
                {
                    WebClient request = new WebClient();
                    request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    if (port.Length > 0 && port != "0") port = ":" + port;
                    byte[] fileData = request.DownloadData("ftp://" + ftpaddress + port + "/" + folder + filename);
                    if (File.Exists(localFilePath + filename) == true)
                    {
                        res = "-Error-Get-" + "File Exist!";
                        return res;
                    }
                    FileStream file = File.Create(localFilePath + filename);
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                    res = "-ok-Get-";
                }
            }
            catch (System.Exception ex) { res = "Error-Get-" + ex.Message.Replace("|", "").Replace("-", "").Replace(";", ""); }
            return res;
        }
        public static string FTPDeleteFile(string ftpUsername, string ftpPassword, string ftpaddress, string folder, string filename, string port = "", string options = "")
        {
            try
            {
                if (port.Length > 0 && port != "0") port = ":" + port;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpaddress + port + "/" + folder + filename);
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return "-ok-Deleted-";
            }
            catch (System.Exception ex) { return "-Error-Deleted-" + ex.Message.Replace("|", "").Replace("-", "").Replace(";", ""); }
        }
        public static string ReadFilesFromFTP(string ftpUsername, string ftpPassword, string ftpaddress, int porti, string options, string FileNameFormat, string destination, string dir)
        {
            string result = "";
            string port = ""; if (porti != 0) port = porti.ToString();
            int count = 0;
            try
            {
                if (FileNameFormat == "") FileNameFormat = "*";
                string[] FileNameFormats = FileNameFormat.Split('|').Where(x => x.Length > 0).ToArray();
                string outmessege = "";
                
                
                if (result.IndexOf("ERR0R") >= 0)
                {
                    {
                       
                       
                        if (dir.Length > 0) { if (dir.Substring(0, 1) != @"/") dir = "/" + dir; if (dir.Substring(dir.Length - 1, 1) != @"/") dir = dir + "/"; }
                        result = "";
                        List<string> all = FTPGetFiles(ftpUsername, ftpPassword, ftpaddress, dir, out outmessege, port, options, FileNameFormat);
                        if (outmessege.IndexOf("-ok-") >= 0)
                        {
                            result = result + outmessege;
                            foreach (string item in all)
                            {
                                string filenamemask = item.Replace("|", "").Replace("-", "").Replace(";", "");
                                string getfileres = "";
                                string optionresult = "";
                                try
                                {
                                    int formatmatch = 0;
                                    foreach (string filenameformat in FileNameFormats)
                                    { formatmatch = 1; if (PatternMatch(item, filenameformat) == true) { formatmatch = 2; break; } }
                                    if (formatmatch == 1) continue;
                                    getfileres = FTPGetFile(ftpUsername, ftpPassword, ftpaddress, dir, destination, item, port, options);

                                    if (getfileres.IndexOf("-ok-") >= 0 && options.ToUpper().IndexOf("DEL") >= 0)
                                    {
                                        try { optionresult = optionresult + FTPDeleteFile(ftpUsername, ftpPassword, ftpaddress, dir, item, port, options); } catch { }
                                    }
                                    count = count + 1;
                                }
                                catch { }
                                result = result + ";" + filenamemask + "|" + getfileres + "|" + optionresult;
                            }
                        }
                        else
                        {
                            result = outmessege.Replace("|", "").Replace("-", "").Replace(";", "");
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                string err = "ERR0R22" + ex.ToString();
                result = result + ex.Message.Replace("|", "").Replace("-", "").Replace(";", "");
            }
            result = "(Server:" + ftpaddress + ",User:" + ftpUsername + ",Port:" + port + ",FileName:" + FileNameFormat + ",Folder:" + destination + ")" + (count > 0 ? count + " ..Files.. Found!" : "No File Found!") + " " + result;
            return result;
        }
    }
}
