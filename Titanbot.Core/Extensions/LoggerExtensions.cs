using Discord;
using Titansmasher.Services.Logging;
using Titansmasher.Services.Logging.Interfaces;

namespace Titanbot.Core.Extensions
{
    public static class LoggerExtensions
    {
        public static void Log(this ILoggerService logger, string area, LogMessage message)
            => logger.Log(message.Severity.ToLogLevel(), area + "/" + message.Source, message.Message);

        public static void Log(this IAreaLogger logger, LogMessage message)
            => logger.Parent.Log(logger.Area, message);

        public static LogLevel ToLogLevel(this LogSeverity severity)
            => (LogLevel)(int)severity;
    }
}