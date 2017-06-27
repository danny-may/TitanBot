using System;
using System.Threading.Tasks;

namespace TitanBot.Logging
{
    public interface ILogger
    {
        void Log(LogSeverity severity, LogType type, string message, string source);
        Task LogAsync(LogSeverity severity, LogType type, string message, string source);
        void Log(ILoggable entry);
        Task LogAsync(ILoggable entry);
        void Log(Exception ex, string source);
        Task LogAsync(Exception ex, string source);
    }
}
