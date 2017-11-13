using System;

namespace Titansmasher.Services.Logging.Interfaces
{
    public interface IAreaLogger
    {
        string Area { get; }
        ILoggerService Parent { get; }

        void Log(LogLevel severity, string message);
        void Log(LogLevel severity, object message);
        void Log(Exception exception);
    }
}