using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignation
{
    public class Utilities
    {
        public static void WriteLogError(Exception ex)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                string logFile = Path.Combine(logPath, "LogFile.txt");
                if (!File.Exists(logFile))
                {
                    File.Create(logFile).Close();
                }

                using (StreamWriter sw = new StreamWriter(logFile, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("g") + ": " + ex.Source + "; " + ex.Message);
                }
            }
            catch
            {

            }
        }

        public static void WriteLogError(string message)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                string logFile = Path.Combine(logPath, "LogFile.txt");
                if (!File.Exists(logFile))
                {
                    File.Create(logFile).Close();
                }

                using (StreamWriter sw = new StreamWriter(logFile, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("g") + ": " + message);
                }
            }
            catch
            {

            }
        }

        public static StringContent StringContentObjectFromJson(string jsonContent)
        {
            StringContent result = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json");
            return result;
        }
    }
}
