using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GetFreeSpace
{
    public static class DiskManager
    {
        [DllImport("kernel32")]
        public static extern int GetDiskFreeSpaceEx(string lpDirectoryName, ref long lpFreeBytesAvailable, ref long lpTotalNumberOfBytes, ref long lpTotalNumberOfFreeBytes);

        public static string GetFreeSpace(string Path, string Options)
        {
            string result30 = "";
            try
            {
                long lpFreeBytesAvailable = 0; long lpTotalNumberOfBytes = 0; long lpTotalNumberOfFreeBytes = 0;
                string bRC = GetDiskFreeSpaceEx(Path, ref lpFreeBytesAvailable, ref lpTotalNumberOfBytes, ref lpTotalNumberOfFreeBytes).ToString();
                if (Options.ToUpper().IndexOf("BYGB") >= 0) bRC = (lpFreeBytesAvailable / (1024 * 1024 * 1024)).ToString(); else if (Options.ToUpper().IndexOf("BYMB") >= 0) bRC = (lpFreeBytesAvailable / (1024 * 1024)).ToString(); else bRC = (lpFreeBytesAvailable / 1024).ToString();
                result30 = bRC;
            }
            catch { }
            return result30;
        }
    }
}
