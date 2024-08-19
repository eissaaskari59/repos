
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConvertHtmlTable2CSV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string csvstring = ConvertHtmlToCSV("<table><tr><td>value1</td><td>value2</td></tr></table>");


        }
        public static string ConvertHtmlToCSV(string htmlinputstring, string excelseparator = ";", string options = "", string lineseperator="")
        {
            if (lineseperator == "") lineseperator = Environment.NewLine;
            try
            {
                htmlinputstring = htmlinputstring.Replace(excelseparator, "");
                if (htmlinputstring.IndexOf("<table") >= 0)
                {
                    int st = htmlinputstring.IndexOf("<table");
                    st = htmlinputstring.IndexOf(">", st);
                    htmlinputstring = htmlinputstring.Substring(st + 1, htmlinputstring.Length - st - 1);
                }
                htmlinputstring = htmlinputstring.Replace("<table>", "");
                htmlinputstring = htmlinputstring.Replace("th>", "td>");
                htmlinputstring = htmlinputstring.Replace("<th", "<td");
                htmlinputstring = htmlinputstring.Replace("<td>", "");
                htmlinputstring = htmlinputstring.Replace("<tr>", "");
                htmlinputstring = htmlinputstring.Replace("</tr>", lineseperator);

                htmlinputstring = htmlinputstring.Replace("</td>", excelseparator);
                htmlinputstring = htmlinputstring.Replace(@"<td style='border: 1px solid gray;'>", "");
                htmlinputstring = htmlinputstring.Replace(@"<td style='border: 1px solid gray'>", "");
                htmlinputstring = htmlinputstring.Replace(@"<td style='border: 1px solid gray;vertical-align:text-top;'>", "");
                htmlinputstring = htmlinputstring.Replace(@"<td style='border: 1px solid grayvertical-align:text-top'>", "");
                htmlinputstring = htmlinputstring.Replace("<h1>", "").Replace("</h1>", lineseperator).Replace("<h2>", "").Replace("</h2>", lineseperator).Replace("<h3>", "").Replace("</h3>", lineseperator);
                htmlinputstring = htmlinputstring.Replace("<br>", lineseperator);
                htmlinputstring = htmlinputstring.Replace("<br/>", lineseperator);
                htmlinputstring = htmlinputstring.Replace("</table>", "");

            }
            catch
            {

            }

            return htmlinputstring;
        }
    }
}
