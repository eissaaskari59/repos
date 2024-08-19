using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItSilentApplicationsFramework;

namespace ConvertHtmlTable2CSV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TrackItSilentApplicationsFramework.Util.DateTimeNowCest();
         
        }
        public static string ConvertHtmlToCSV(string result, string excelseparator = ";", string options = "", string lineseperator="")
        {
            if (lineseperator == "") lineseperator = Environment.NewLine;
            try
            {
                result = result.Replace(excelseparator, "");
                if (result.IndexOf("<table") >= 0)
                {
                    int st = result.IndexOf("<table");
                    st = result.IndexOf(">", st);
                    result = result.Substring(st + 1, result.Length - st - 1);
                }
                result = result.Replace("<table>", "");
                result = result.Replace("th>", "td>");
                result = result.Replace("<th", "<td");
                result = result.Replace("<td>", "");
                result = result.Replace("<tr>", "");
                result = result.Replace("</tr>", lineseperator);

                result = result.Replace("</td>", excelseparator);
                result = result.Replace(@"<td style='border: 1px solid gray;'>", "");
                result = result.Replace(@"<td style='border: 1px solid gray'>", "");
                result = result.Replace(@"<td style='border: 1px solid gray;vertical-align:text-top;'>", "");
                result = result.Replace(@"<td style='border: 1px solid grayvertical-align:text-top'>", "");
                result = result.Replace("<h1>", "").Replace("</h1>", lineseperator).Replace("<h2>", "").Replace("</h2>", lineseperator).Replace("<h3>", "").Replace("</h3>", lineseperator);
                result = result.Replace("<br>", lineseperator);
                result = result.Replace("<br/>", lineseperator);
                result = result.Replace("</table>", "");

            }
            catch
            {

            }

            return result;
        }
    }
}
