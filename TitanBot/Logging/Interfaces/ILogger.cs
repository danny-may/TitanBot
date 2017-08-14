using System;

namespace TitanBot.Logging
{
    public interface ILogger
    {
        void Log(LogSeverity severity, LogType type, string message, string source);
        void Log(ILoggable entry);
        void Log(Exception ex, string source);
    }
}
