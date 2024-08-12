using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSplitters
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string result = GetSplitItemBySplitter("value1|value2", "|`0");//result will be value1;
            List<string> resultarr = SplitByLength("123456", 2); //result will be 12 34 56

        }
        public static string GetSettingFromString(ref string setting, string key, string defaultvalue = "", byte AddMissingKey = 0, string seperator = "|")
        {
            string value = defaultvalue;
            try
            {
                string setting1 = setting.ToUpper();
                setting1 = setting1 + seperator.ToUpper(); if (key.IndexOf(":") < 0 && AddMissingKey != 2) { key = key + ":"; }
                int st = setting1.IndexOf(key.ToUpper());
                if (st >= 0) { st = st + key.Length; int en = setting1.IndexOf(seperator.ToUpper(), st); value = setting.Substring(st, en - st); if (value == "") value = defaultvalue; }
                else if (AddMissingKey == 1) { setting = setting + key + defaultvalue + seperator; }
            }
            catch { }

            return value;
        }
        public static string GetSplitItemBySplitter(string Value, string Splitter_Index, string options55 = "", string joinby55 = "")
        {
            string variableresult = "";
            try
            {
                string SplitItemsplitterstr = Splitter_Index.Split('`')[0];
                char[] SplitItemsplitter = SplitItemsplitterstr.ToCharArray();

                try
                {
                    int SplitItemsindex = -1; try { SplitItemsindex = int.Parse(Splitter_Index.Split('`')[1]); } catch { }
                    string[] SplitItemsarr = Value.Split(SplitItemsplitter);
                    if (SplitItemsplitterstr.Length > 1)
                    {
                        SplitItemsarr = Value.Replace("`", "").Replace(SplitItemsplitterstr, "`").Split('`');
                        if (options55.ToUpper().IndexOf("NOTADDSPLITTER") < 0) for (int i = 0; i < SplitItemsarr.Length; i++) SplitItemsarr[i] = SplitItemsplitterstr + SplitItemsarr[i];
                    }
                    if (SplitItemsindex >= 0)
                    {
                        if (options55.ToUpper().IndexOf("FROMEND") >= 0)
                        {
                            SplitItemsindex = SplitItemsarr.Length - SplitItemsindex - 1;
                        }
                        if (options55.ToUpper().IndexOf("BASE1") >= 0) SplitItemsindex = SplitItemsindex - 1;
                        variableresult = SplitItemsarr[SplitItemsindex];
                    }
                    else
                    {
                        if (options55.IndexOf("ADDCOUNTER") >= 0)
                        {
                            variableresult = "";
                            for (int i = 0; i < SplitItemsarr.Length; i++) { variableresult = variableresult + (i + 1).ToString() + "`" + SplitItemsarr[i] + joinby55; }
                        }
                        else if (options55.IndexOf("SAVEITEMSASLABEL") >= 0)
                        {
                            variableresult = "";
                            for (int i = 0; i < SplitItemsarr.Length; i++) { variableresult = variableresult + SplitItemsarr[i] + joinby55; }
                        }

                        else if (options55.IndexOf("GroupByField") >= 0)
                        {

                            string GroupByField = GetSettingFromString(ref options55, "GroupByField:", "|", 0, "/");
                            string Index = GetSettingFromString(ref options55, "Index:", "|", 0, "/");
                            List<KeyValuePair<string, string>> all = new List<KeyValuePair<string, string>>();
                            List<string> keys = all.Select(x => x.Key).Distinct().ToList();
                            variableresult = "";

                            foreach (string item in keys)
                            {
                                try
                                {
                                    string thisitem = "";
                                    List<KeyValuePair<string, string>> thiss = all.Where(x => x.Key == item).ToList();
                                    try
                                    {
                                        if (Index == "Last") thisitem = thiss[thiss.Count - 1].Value;
                                        else if (Index == "All") thisitem = string.Join("", thiss.Select(x => x.Value));
                                        else { thisitem = thiss[int.Parse(Index)].Value; }
                                    }
                                    catch { }
                                    variableresult = variableresult + thisitem + joinby55;
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            variableresult = string.Join(joinby55, SplitItemsarr);
                        }
                    }
                }
                catch (Exception ex)
                {

                    variableresult = "";
                }


            }
            catch { }
            return variableresult;

        }
        public static List<string> SplitByLength(string item, int len)
        {
            List<string> list = new List<string>();
            try
            {
                string thisitem = ""; int counter = 0;
                for (int i = 0; i < item.Length; i++) { thisitem = thisitem + item.Substring(i, 1); counter = counter + 1; if (counter >= len) { counter = 0; list.Add(thisitem); thisitem = ""; } }
                list.Add(thisitem);
            }
            catch { }
            return list;
        }
        // split a text file to files with specific length
        public static string SplitFileBYLINES(List<string> items, string options, int SPLITBYLINES, Encoding encode)
        {
            string res = "";
            foreach (string item in items)
                try
                {

                    string[] lines = File.ReadAllLines(item, encode);

                    if (lines.Length > SPLITBYLINES + 3)
                    {
                        int counter1 = 1; int counter2 = 1;
                        string currentfile = Path.GetDirectoryName(item) + @"\" + Path.GetFileNameWithoutExtension(item) + "_IMSPLITTED" + counter2.ToString("0000") + Path.GetExtension(item);
                        res = res + ", " + item + " Splitted to " + currentfile;
                        StreamWriter sw = new StreamWriter(currentfile, false, encode);
                        foreach (string line in lines)
                        {
                            sw.WriteLine(line);

                            counter1 = counter1 + 1;
                            if (counter1 > SPLITBYLINES)
                            {
                                counter2 = counter2 + 1;
                                counter1 = 1;
                                currentfile = Path.GetDirectoryName(item) + @"\" + Path.GetFileNameWithoutExtension(item) + "_IMSPLITTED" + counter2.ToString("0000") + Path.GetExtension(item);
                                sw.Close();
                                sw = new StreamWriter(currentfile, false, encode);
                                res = res + " , " + currentfile;
                            }
                        }
                        try { sw.Close(); } catch { }
                    }
                }
                catch (System.Exception ex) { res = res + "ERR0R86:" + ex.Message; }
            return res;

        }
    }
}
