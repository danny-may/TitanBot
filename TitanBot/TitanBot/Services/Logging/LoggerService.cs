using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using TitanBot.Core.Services.Logging;
using Discord.WebSocket;
using System.IO;
using TitanBot.Utility;

namespace TitanBot.Services.Logging
{
    public class LoggerService : ILoggerService
    {
        protected readonly DiscordSocketClient Discord;

        protected readonly string LogPath = $"./logs/{DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss")}.txt";
        protected FileInfo LogFile => new FileInfo(Path.Combine(AppContext.BaseDirectory, LogPath));

        protected readonly ProcessingQueue Queue = new ProcessingQueue();

        public LoggerService(DiscordSocketClient discord)
        {
            Discord = discord;

            Discord.Log += LogAsync;
        }

        public async void Log(ILoggable entry)
            => await LogAsync(entry);

        public async void Log(Exception error)
            => await LogAsync(error);

        public async void Log(Exception error, string source)
            => await LogAsync(error, source);

        public async void Log(LogMessage message)
            => await LogAsync(message);

        public Task LogAsync(ILoggable entry)
        {
            throw new NotImplementedException();
        }

        public Task LogAsync(Exception error)
        {
            throw new NotImplementedException();
        }

        public Task LogAsync(Exception error, string source)
        {
            throw new NotImplementedException();
        }

        public Task LogAsync(LogMessage message)
            => LogString($"{DateTime.UtcNow.ToString("hh:mm:ss")} [{message.Severity}] {message.Source}: {message.Exception?.ToString() ?? message.Message}");

        private Task LogString(string message)
            => Queue.Run(async () =>
            {
                if (!LogFile.Directory.Exists)
                    LogFile.Directory.Create();
                if (!LogFile.Exists)
                    LogFile.Create().Dispose();

                File.AppendAllText(LogFile.FullName, message + "\n");

                await Console.Out.WriteLineAsync(message);
            });
    }
}