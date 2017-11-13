using System;

namespace Titanbot.Core.Logging.Interfaces
{
    public interface ILogger
    {
        void Log(LogSeverity severity, string area, string message);
        void Log(LogSeverity severity, string area, object message);
        void Log(string area, Exception exception);

        IAreaLogger CreateAreaLogger(string area);
        IAreaLogger CreateAreaLogger<TArea>();

        event Action<string> OnLog;
    }
}