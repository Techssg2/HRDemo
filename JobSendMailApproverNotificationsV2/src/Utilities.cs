using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src
{
    public class Utilities
    {
        private static string GetLogFilePath()
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            string fileName = $"LogFile_{DateTime.Now:yyyy-MM-dd}.txt";
            return Path.Combine(logPath, fileName);
        }

        public static void WriteLogError(Exception ex)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(GetLogFilePath(), true))
                {
                    sw.WriteLine(DateTime.Now.ToString("g") + ": " + ex.Source + "; " + ex.Message);
                }
            }
            catch
            {
                using (StreamWriter sw = new StreamWriter(GetLogFilePath(), true))
                {
                    sw.WriteLine(DateTime.Now.ToString("g") + ": " + ex.Source + "; " + ex.Message);
                }
            }
        }

        public static void WriteLogError(string message)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(GetLogFilePath(), true))
                {
                    sw.WriteLine(DateTime.Now.ToString("g") + ": " + message);
                }
            }
            catch
            {
                using (StreamWriter sw = new StreamWriter(GetLogFilePath(), true))
                {
                    sw.WriteLine(DateTime.Now.ToString("g") + ": " + message);
                }
            }
        }

        public static void CleanupOldLogs(int keepDays = 30)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logPath))
                    return;

                DateTime cutoffDate = DateTime.Now.AddDays(-keepDays);
                string[] logFiles = Directory.GetFiles(logPath, "LogFile_*.txt");

                foreach (string file in logFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(file);
                        WriteLogError($"Deleted old log file: {Path.GetFileName(file)}");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogError($"Error cleaning up old logs: {ex.Message}");
            }
        }

        public static StringContent StringContentObjectFromJson(string jsonContent)
        {
            StringContent result = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json");
            return result;
        }
    }
}
