using Discord;
using System.Threading.Tasks;
using Titansmasher.Services.Logging;
using Titansmasher.Services.Logging.Interfaces;

namespace Titanbot.Core.Extensions
{
    public static class LoggerExtensions
    {
        public static void Log(this ILoggerService logger, LogMessage message, string area = null)
            => logger.Log(message.Severity.ToLogLevel(), message.Message, message.Source);

        public static Task LogAsync(this ILoggerService logger, LogMessage message, string area = null)
        {
            logger.Log(message, area);
            return Task.CompletedTask;
        }

        public static LogLevel ToLogLevel(this LogSeverity severity)
            => (LogLevel)(int)severity;
    }
}