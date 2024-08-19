using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProcessManager
{
    public static class  Program
    {
        static void Main(string[] args)
        {
            KillProcessAndChildren(1000);
            GetCommandLine(Process.GetCurrentProcess());
            GetRunningProcesses("server");

        }
        public static string GetCommandLine(this Process process)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
                using (ManagementObjectCollection objects = searcher.Get()) { return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString(); }
            }
            catch { return ""; }
        }
        public static string KillProcessAndChildren(int pid)
        {
            string res = "";
            if (pid == 0) { return ""; }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc) { res = res + " " + KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"])); }
            try { Process proc = Process.GetProcessById(pid); proc.Kill(); res = res + " " + pid + " Killed!"; }
            catch (ArgumentException) { res = res + " " + pid + " Not Found!"; }
            return res;
        }
        public static string GetRunningProcesses(string server = "", string file = "", string options = "",string SystemMail="")
        {
            if (server == "") server = Environment.MachineName;
            if (file == "") file = "IntegrationManager".ToUpper(); else file = Path.GetFileNameWithoutExtension(file).ToUpper();
            string runningprocesses = "";
            try
            {
                
                List<Process> allprocesses = Process.GetProcesses(server).ToList();
                List<Process> processes = allprocesses.Where(x => x.ProcessName.ToUpper().IndexOf(file) >= 0).ToList();
                if (options.IndexOf("RUNNINGPROCESSCOUNT") >= 0) { return processes.Count().ToString(); }
                if (options.IndexOf("RETURNPIDS") >= 0) { return string.Join(";", processes.Select(x => x.Id)); }
                try { processes = processes.OrderBy(x => x.StartTime).ToList(); } catch { processes = processes.OrderBy(x => x.Id).ToList(); }
                runningprocesses = "<br> Running now :" + processes.Count + " IMs:<br/><table><tr><th>PID</th><th>StartTime</th><th>RunningSecods</th><th>Arg</th></tr>";
                foreach (Process item in processes)
                {
                    
                    string sts = ""; DateTime strt = DateTime.Now; try { strt = item.StartTime; sts = strt.ToString(); } catch { }
                    runningprocesses = runningprocesses + "<tr>" + GetFormattedTableCell("<a href='mailto:" + SystemMail + "?subject=SHIPMENTS 0 0 0 KILLP:" + item.Id + "'> Kill " + item.Id + "</a>", 2)  + GetFormattedTableCell(strt.ToString(), 2) + GetFormattedTableCell((((int)(DateTime.Now - strt).TotalSeconds)).ToString(), 2) + "</tr>";
                }
            }
            catch (System.Exception ex) { string s = ex.Message; }
            return runningprocesses;
        }
        public static string GetFormattedTableCell(string item, int type = 0, string options = "")
        {
            if (options.Length > 0) { if (options.IndexOf("REMOVEBREAK") >= 0) { item = item.Replace(Environment.NewLine, " ").Replace("<br>", ""); } }
            
            string res = "";

            try
            {
                switch (type)
                {
                    case 2:
                        res = res + "<td>" + item + "</td>";
                        break;
                    case 1:
                        res = res + "<td style='vertical-align:text-top;border: 1px dotted blue;'>" + item + "</td>";
                        break;
                    case 3:
                        res = res + "<td style='border: 1px solid gray;'>" + item + "</td>";
                        break;
                    case 4:
                        res = res + "<td style='border: 1px solid gray;background-color:gray;'>" + item + "</td>";
                        break;
                    case 5:
                        res = res + "<td style='border: 1px solid gray;background-color:green;'>" + item + "</td>";
                        break;
                    case 6:
                        res = res + "<td colspan='2' style='border: 1px solid gray;width=600px;'>" + item + "</td>";
                        break;
                    case 7:
                        res = res + "<th>" + item + "</th>";
                        break;
                    default:
                        res = res + "<td style='border: 1px solid gray;vertical-align:text-top;'>" + item + "</td>";
                        break;
                }
            }
            catch { }

            return res;

        }

    }
}
