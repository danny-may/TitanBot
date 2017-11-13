using System;

namespace Titanbot.Core.Logging.Interfaces
{
    public interface IAreaLogger
    {
        void Log(LogSeverity severity, string message);
        void Log(LogSeverity severity, object message);
        void Log(Exception exception);
    }
}