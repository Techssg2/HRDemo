using System;
using System.IO;

namespace SyncOrgchartJob
{
    public class Logger
    {
        public static void Write(string message, bool isShowConsole = false)
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var logDir = Path.Combine(baseDir, "log");

                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                var timestamp = DateTime.Now.ToString("dd-MM-yyyy");
                var logFile = Path.Combine(logDir, $"log_{timestamp}.txt");

                using (var writer = File.AppendText(logFile))
                {
                    writer.WriteLine($"{DateTime.Now:HH:mm:ss} - {message}");
                    if (isShowConsole)
                    {
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // In case writing log fails, don't throw further
                Console.WriteLine("Logging failed: " + ex.Message);
            }
        }
    }
}