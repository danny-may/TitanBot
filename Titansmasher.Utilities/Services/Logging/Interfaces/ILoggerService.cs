using System;

namespace Titansmasher.Services.Logging.Interfaces
{
    public interface ILoggerService
    {
        void Log(LogLevel severity, string message, string area = null);
        void Log(LogLevel severity, object message, string area = null);
        void Log(Exception exception, string area = null);

        void ClearOld(DateTime before = default(DateTime));

        event Action<string> OnLog;
        LogLevel Scope { get; set; }
    }
}