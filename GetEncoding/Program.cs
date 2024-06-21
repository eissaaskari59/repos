using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetEncoding
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }
        public static Encoding GetEncoding(int EncodingCode, string EncodingName = "")
        {
            EncodingName = EncodingName.ToUpper(); Encoding encoding = null;
            
            if (EncodingCode == GetFileEncodingASCII || EncodingName == "ASCII") { encoding = Encoding.ASCII; }
            else if (EncodingCode == GetFileEncodingUNICODE || EncodingName == "Unicode".ToUpper()) { encoding = Encoding.Unicode; }
            else if (EncodingCode == 1252 || EncodingName == "1252".ToUpper()) { encoding = Encoding.GetEncoding(1252); }
            else if (EncodingCode == GetFileEncodingUTF8 || EncodingName == "UTF8") { encoding = Encoding.UTF8; }
            else if (EncodingCode == GetFileEncodingUTF32 || EncodingName == "UTF32") { encoding = Encoding.UTF32; }
            else if (EncodingCode == GetFileEncodingUTF7 || EncodingName == "UTF7") { encoding = Encoding.UTF7; }
            else if (EncodingCode == GetFileEncodingDefault || EncodingName == "DEFAULT") { encoding = Encoding.Default; }
            else { encoding = Encoding.Default; }
            return encoding;
        }
        public static int GetFileEncodingASCII = 0;
        public static int GetFileEncodingUNICODE = 1;
        public static int GetFileEncodingUTF8 = 2;
        public static int GetFileEncodingUTF32 = 3;
        public static int GetFileEncodingUTF7 = 4;
        public static int GetFileEncodingDefault = 5;

    }
}
