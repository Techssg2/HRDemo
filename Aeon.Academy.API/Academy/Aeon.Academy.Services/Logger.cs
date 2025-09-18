using NLog;
using System;

namespace Aeon.Academy.Services
{
    public class Logger : ILogger
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        public void LogDebug(string message)
        {
            logger.Debug(message);
        }
        public void LogError(string message)
        {
            logger.Error(message);
        }
        public void LogError(Exception exception)
        {
            logger.Error(GetErrorMessage(exception));
        }
        public void LogInfo(string message)
        {
            logger.Info(message);
        }
        public void LogWarn(string message)
        {
            logger.Warn(message);
        }
        private string GetErrorMessage(Exception ex)
        {
            Exception innerException = ex.InnerException;

            var errorMessage = ex.Message + " - StackTrace: " + ex.StackTrace;
            while (innerException != null)
            {
                errorMessage = errorMessage + "<!-- Inner Exception: {0} -->" + innerException.Message + " - StackTrace: " + innerException.StackTrace;
                innerException = innerException.InnerException;
            }

            return errorMessage;
        }
    }
}
