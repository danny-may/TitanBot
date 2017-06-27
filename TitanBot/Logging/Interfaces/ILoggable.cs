using System;

namespace TitanBot.Logging
{
    public interface ILoggable
    {
        LogSeverity Severity { get; }
        LogType LogType { get; }
        string Message { get; }
        string Source { get; }
        DateTime LogTime { get; }
    }
}
