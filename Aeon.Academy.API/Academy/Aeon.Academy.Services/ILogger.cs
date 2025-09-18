using System;

namespace Aeon.Academy.Services
{
    public interface ILogger
    {
        void LogDebug(string message);
        void LogError(string message);
        void LogError(Exception exception);
        void LogInfo(string message);
        void LogWarn(string message);
    }
}
