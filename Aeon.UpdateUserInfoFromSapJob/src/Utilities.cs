using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.UpdateUserInfoFromSapJob.src
{
    public class Utilities
    {
        public static void WriteLogError(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
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
                Console.WriteLine("WriteLogError.message: " + message);
                string directoryLog = @ConfigurationManager.AppSettings["Logs"] + "LogFile_" + DateTimeOffset.Now.ToString("ddMMyyyy") + ".txt";
                Console.WriteLine("directoryLog2: " + directoryLog);
                if (!System.IO.File.Exists(@directoryLog))
                {
                    System.IO.File.Create(@directoryLog).Dispose();
                }
                sw = new StreamWriter(@directoryLog, true);
                sw.WriteLine(DateTime.Now.ToString("g") + " " + message);
            }
            catch (Exception e)
            {
                sw.WriteLine("Error at WriteLogError function: " + DateTime.Now.ToString("g") + ": " + e.Message);
            }
            finally
            {
                sw.Flush();
                sw.Close();
            }
        }
        public static StringContent StringContentObjectFromJson(string jsonContent)
        {
            StringContent result = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json");
            return result;
        }
    }
}
