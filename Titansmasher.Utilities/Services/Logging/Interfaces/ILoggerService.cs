using System;

namespace Titansmasher.Services.Logging.Interfaces
{
    public interface ILoggerService
    {
        void Log(LogLevel severity, string area, string message);
        void Log(LogLevel severity, string area, object message);
        void Log(string area, Exception exception);

        IAreaLogger CreateAreaLogger(string area);
        IAreaLogger CreateAreaLogger<TArea>();

        void ClearOld(DateTime before = default(DateTime));

        event Action<string> OnLog;
        LogLevel Scope { get; set; }
    }
}