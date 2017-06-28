﻿using System;
using System.IO;
using System.Threading.Tasks;
using TitanBot.Util;

namespace TitanBot.Logging
{
    public class Logger : ILogger
    {
        private readonly string LogPath;
        private readonly object SyncLock = new object();
        protected virtual LogSeverity LogLevel => LogSeverity.Critical | LogSeverity.Info | LogSeverity.Error;

        public Logger() : this($@".\logs\{FileUtil.GetTimestamp()}.log") { }
        public Logger(string logLocation)
            => LogPath = FileUtil.GetAbsolutePath(logLocation);             

        public void Log(ILoggable entry)
        {
            if (!ShouldLog(entry.Severity))
                return;
            lock (SyncLock)
            {
                WriteLog(entry);
            }
        }

        protected virtual void WriteLog(ILoggable entry)
        {
            FileUtil.EnsureDirectory(LogPath);
            File.AppendAllText(LogPath, entry.ToString());
        }

        protected virtual bool ShouldLog(LogSeverity severity)
            => (LogLevel & severity) != 0;

        public void Log(LogSeverity severity, LogType type, string message, string source)
            => Log(new LogEntry(severity, type, message, source));

        public void Log(Exception ex, string source)
            => Log(new LogEntry(LogSeverity.Error, LogType.Exception, ex.ToString(), source));

        public Task LogAsync(ILoggable entry)
            => Task.Run(() => Log(entry));

        public Task LogAsync(LogSeverity severity, LogType type, string message, string source)
            => Task.Run(() => Log(severity, type, message, source));

        public Task LogAsync(Exception ex, string source)
            => Task.Run(() => Log(ex, source));
    }
}