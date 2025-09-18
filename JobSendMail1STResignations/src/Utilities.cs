using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignations.src
{
    public class Utilities
    {
        public static void WriteLogError(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                string logFile = "LogFile_" + DateTimeOffset.Now.ToString("yyyMMdd");
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + ("\\Logs\\" + logFile + ".txt"), true);
                sw.WriteLine(DateTime.Now.ToString("g") + ": " + ex.Source + "; " + ex.Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }
        public static void WriteLogError(string message)
        {
            StreamWriter sw = null;
            try
            {
                string logFile = "LogFile_" + DateTimeOffset.Now.ToString("yyyMMdd");
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + ("\\Logs\\" + logFile + ".txt"), true);
                sw.WriteLine(DateTime.Now.ToString("g") + ": " + message);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }
    }
}
