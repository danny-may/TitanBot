using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Core.Services.Logging
{
    public interface ILoggerService
    {
        void Log(ILoggable entry);

        Task LogAsync(ILoggable entry);

        void Log(Exception error);

        Task LogAsync(Exception error);

        void Log(Exception error, string source);

        Task LogAsync(Exception error, string source);

        void Log(LogMessage message);

        Task LogAsync(LogMessage message);
    }
}