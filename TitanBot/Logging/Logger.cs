using System;
using System.IO;
using TitanBot.Helpers;

namespace TitanBot.Logging
{
    public class Logger : ILogger
    {
        private readonly string LogPath;
        private readonly object SyncLock = new object();
        protected virtual LogSeverity LogLevel => LogSeverity.Critical | LogSeverity.Info | LogSeverity.Error;
        protected SynchronisedExecutor SyncLog { get; } = new SynchronisedExecutor();

        public Logger() : this($@".\logs\{FileUtil.GetTimestamp()}.log") { }
        public Logger(string logLocation)
            => LogPath = FileUtil.GetAbsolutePath(logLocation);

        public void Log(ILoggable entry)
        {
            if (!ShouldLog(entry.Severity))
                return;
            SyncLog.Run(() => WriteLog(entry));
        }

        protected virtual void WriteLog(ILoggable entry)
        {
            FileUtil.EnsureDirectory(LogPath);
            File.AppendAllText(LogPath, entry.ToString() + "\n");
        }

        protected virtual bool ShouldLog(LogSeverity severity)
            => LogLevel > severity;

        public void Log(LogSeverity severity, LogType type, string message, string source)
            => Log(new LogEntry(severity, type, message, source));

        public void Log(Exception ex, string source)
            => Log(new LogEntry(LogSeverity.Error, LogType.Exception, ex.ToString(), source));
    }
}
