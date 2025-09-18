using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Reflection;
using System.Collections;
using Microsoft.Win32;
using System.IO;
using System.Security;
using System.Collections.ObjectModel;

namespace  Aeon.HR.UI
{
    public class Logging
    {

        internal string LogFile = null;
        internal string LineToLog = null;

        /// <summary>
        /// This will actually log the content to the actual file.
        /// </summary>
        /// <remarks>Should be called by the <c>SPSecurity.RunWithElevatedPrivileges</c></remarks>
        internal void Write()
        {
            try
            {
                string actualfilename = Logging.GetCurrentLogFile(this.LogFile);
                using (System.IO.StreamWriter file = System.IO.File.AppendText(actualfilename))
                {
                    file.WriteLine(this.LineToLog);
                    file.Flush();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Log an exception to <c>logfile</c>
        /// </summary>
        /// <param name="logfile">the base name of the logfile to log to logfile-datetimestamp.log</param>
        /// <param name="exc">The exception to log</param>
        public static void LogException(string logfile, Exception exc)
        {
            try
            {
                string line = string.Format("{0}\r\n{1}", exc.Message, exc.StackTrace);
                Logging.LogMessage(logfile, exc.GetType().ToString(), line);
            }
            catch
            {
            }
        }
        /// <summary>
        /// Log a message to <c>logfile</c>
        /// </summary>
        /// <param name="logfile">the base name of the logfile to log to logfile-datetimestamp.log</param>
        /// <param name="category">a category to log</param>
        /// <param name="message">the message to log</param>
        public static void LogMessage(string category, string message)
        {
            LogMessage(Logging.NameFileLog, category, message);
        }

        public static void LogMessage(string logfile, string category, string message)
        {
            try
            {
                if (logfile != null)
                {
                    Logging l = new Logging();
                    l.LogFile = logfile;
                    l.LineToLog = string.Format("{0}\t{1}\t{2}",
                            DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                            category.PadLeft(25),
                            message);
                    // this way we can always write to the logfiles
                    SPSecurity.CodeToRunElevated log = new SPSecurity.CodeToRunElevated(l.Write);
                    SPSecurity.RunWithElevatedPrivileges(log);
                }
            }
            catch
            {
            }
        }

        private const string LogExtention = ".log";

        /// <summary>
        /// Parse/handle/cleanup the logfiles for the baselogname <c>logfile</c>
        /// </summary>
        /// <param name="logfile">the basename for the logfiles</param>
        /// <returns>the actual filename to log to</returns>
        private static string GetCurrentLogFile(string logfile)
        {
            string currentLogFile = "";

            try
            {
                string sLogLocation = GetLogsLocation();

                string timestamp = DateTime.Now.AddMinutes(-SPDiagnosticsService.Local.LogCutInterval).ToString("yyyyMMdd-HHmm");

                string lastallowedlogfilename = System.IO.Path.Combine(
                    sLogLocation,
                     logfile + "-" + timestamp + Logging.LogExtention);
                // get existing logfiles
                string[] files = System.IO.Directory.GetFiles(sLogLocation, logfile + "*.log", System.IO.SearchOption.TopDirectoryOnly);

                System.Collections.ArrayList list = new System.Collections.ArrayList();
                list.AddRange(files);

                // clean up old log files
                if (list.Count > SPDiagnosticsService.Local.DaysToKeepLogs)
                {
                    int count = list.Count;
                    while (count > SPDiagnosticsService.Local.DaysToKeepLogs)
                    {
                        System.IO.File.Delete(list[0] as string);
                        count--;
                    }
                }
                // lookup current logfile

                if (list.Count > 0)
                {
                    string lastfile = list[list.Count - 1] as string;
                    if (string.Compare(lastfile, lastallowedlogfilename) > 0)
                        currentLogFile = lastfile;
                }
                // create a new logfile
                if (currentLogFile == "")
                {
                    string timestampn = DateTime.Now.ToString("yyyyMMdd-HHmm");
                    currentLogFile = System.IO.Path.Combine(
                        sLogLocation,
                         logfile + "-" + timestampn + Logging.LogExtention);
                }
            }
            catch
            {
            }

            return currentLogFile;
        }

        public static string GetLogsLocation()
        {
            string logLocation = String.Empty;
            if (IsWSSInstalled)
            {
                logLocation = GetSPDiagnosticsLogLocation();
                if (logLocation == String.Empty)
                    logLocation = GetStandardLogLocation();
            }

            logLocation = Environment.ExpandEnvironmentVariables(logLocation);

            return logLocation;
        }

        private static string GetStandardLogLocation()
        {
            string logLocation = WSSInstallPath;
            if (logLocation != String.Empty)
                logLocation = Path.Combine(logLocation, "logs");

            return logLocation;
        }

        private static string GetSPDiagnosticsLogLocation()
        {
            string logLocation = String.Empty;
            Type diagSvcType = null;
            if (Logging.SPVersion == SPVersion.SP2016)
                diagSvcType = Type.GetType("Microsoft.SharePoint.Administration.SPDiagnosticsService, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            else if (Logging.SPVersion == SPVersion.SP2013)
                diagSvcType = Type.GetType("Microsoft.SharePoint.Administration.SPDiagnosticsService, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            else if (Logging.SPVersion == SPVersion.SP2010)
                diagSvcType = Type.GetType("Microsoft.SharePoint.Administration.SPDiagnosticsService, Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            else if (Logging.SPVersion == SPVersion.SP2007)
                diagSvcType = Type.GetType("Microsoft.SharePoint.Administration.SPDiagnosticsService, Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            if (diagSvcType != null)
            {
                PropertyInfo propLocalDiagSvc = diagSvcType.GetProperty("Local", BindingFlags.Public | BindingFlags.Static);
                object localDiagSvc = propLocalDiagSvc.GetValue(null, null);
                PropertyInfo property = localDiagSvc.GetType().GetProperty("LogLocation");
                logLocation = (string)property.GetValue(localDiagSvc, null);
            }

            return logLocation;
        }
        static IList<TraceSeverity> severities = new List<TraceSeverity>((IEnumerable<TraceSeverity>)Enum.GetValues(typeof(TraceSeverity)));

        public static SPVersion SPVersion
        {
            get
            {
                try
                {
                    var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\16.0");
                    if (key != null)
                        return SPVersion.SP2016;

                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\15.0");
                    if (key != null)
                        return SPVersion.SP2013;

                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\14.0");
                    if (key != null)
                        return SPVersion.SP2010;

                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\12.0");
                    if (key != null)
                        return SPVersion.SP2007;

                }
                catch (SecurityException) { }
                return SPVersion.Unknown;
            }
        }

        public static bool IsWSSInstalled
        {
            get
            {
                try
                {
                    RegistryKey key = GetWSSRegistryKey();
                    if (key != null)
                    {
                        object val = key.GetValue("SharePoint");
                        if (val != null && val.Equals("Installed"))
                            return true;
                    }
                }
                catch (SecurityException) { }
                return false;
            }
        }

        public static bool IsMOSSInstalled
        {
            get
            {
                try
                {
                    using (RegistryKey key = GetMOSSRegistryKey())
                        if (key != null)
                        {
                            string versionStr = key.GetValue("BuildVersion") as string;
                            if (versionStr != null)
                            {
                                Version buildVersion = new Version(versionStr);
                                if (buildVersion.Major == 12 || buildVersion.Major == 14 || buildVersion.Major == 15)
                                    return true;
                            }
                        }
                }
                catch (SecurityException) { }
                return false;
            }
        }

        public static string LatestLogFile
        {
            get
            {
                string lastAccessedFile = null;
                if (IsWSSInstalled)
                    lastAccessedFile = GetLastAccessedFile(GetLogsLocation());

                return lastAccessedFile;
            }
        }
        public static string WSSInstallPath
        {
            get
            {
                string installPath = String.Empty;
                try
                {
                    using (RegistryKey key = GetWSSRegistryKey())
                        if (key != null)
                            installPath = key.GetValue("Location").ToString();
                }
                catch (SecurityException) { }
                return installPath;
            }
        }

        public static ICollection TraceSeverities
        {
            get
            {
                return new ReadOnlyCollection<TraceSeverity>(severities);
            }
        }

        public static int GetSeverity(string level)
        {
            try
            {
                var severity = (TraceSeverity)Enum.Parse(typeof(TraceSeverity), level, true);
                return (int)severity;
            }
            catch (ArgumentException)
            {
                return 0;
            }
        }

        public static string GetLastAccessedFile(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                var dirInfo = new DirectoryInfo(folderPath);
                var file = dirInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
                if (file != null)
                    return file.FullName;
            }
            return null;
        }

        public static IEnumerable<string> GetServerNames()
        {
            Type farmType = null;
            if (Logging.SPVersion == SPVersion.SP2016)
                farmType = Type.GetType("Microsoft.SharePoint.Administration.SPFarm, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            else if (Logging.SPVersion == SPVersion.SP2013)
                farmType = Type.GetType("Microsoft.SharePoint.Administration.SPFarm, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            else if (Logging.SPVersion == SPVersion.SP2010)
                farmType = Type.GetType("Microsoft.SharePoint.Administration.SPFarm, Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            else if (Logging.SPVersion == SPVersion.SP2007)
                farmType = Type.GetType("Microsoft.SharePoint.Administration.SPFarm, Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");

            if (farmType != null)
            {
                PropertyInfo propLocalFarm = farmType.GetProperty("Local", BindingFlags.Public | BindingFlags.Static);
                object localFarm = propLocalFarm.GetValue(null, null);
                PropertyInfo propServers = localFarm.GetType().GetProperty("Servers", BindingFlags.Public | BindingFlags.Instance);
                IEnumerable servers = (IEnumerable)propServers.GetValue(localFarm, null);
                foreach (object server in servers)
                {
                    PropertyInfo propServerName = server.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                    string serverName = (string)propServerName.GetValue(server, null);
                    yield return serverName;
                }
            }
        }

        static RegistryKey GetMOSSRegistryKey()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Office Server\16.0");
            if (key == null)
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Office Server\15.0");
            if (key == null)
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Office Server\14.0");
            if (key == null)
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Office Server\12.0");
            return key;
        }

        static RegistryKey GetWSSRegistryKey()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\16.0");
            if (key == null)
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\15.0");
            if (key == null)
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\14.0");
            if (key == null)
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\12.0");
            return key;
        }
        public const string NameFileLog = "SSG_2019_Log";
    }

    public static class LoggingExtention
    {
        public static void LogMessage(this object sender, string message)
        {
            try
            {
                if (sender != null)
                    Logging.LogMessage(sender.GetType().FullName, message);
            }
            catch
            {
            }
        }

        public static void LogMessage(this object sender, Exception exception)
        {
            try
            {
                if (exception != null)
                    LogMessage(sender, exception.ToString());
            }
            catch
            {
            }
        }
    }

    public enum SPVersion
    {
        Unknown,
        SP2007,
        SP2010,
        SP2013,
        SP2016
    }

    public enum TraceSeverity
    {
        Verbose,
        Information,
        Warning,
        Medium,
        High,
        CriticalEvent,
        Exception,
        Unexpected
    }

}