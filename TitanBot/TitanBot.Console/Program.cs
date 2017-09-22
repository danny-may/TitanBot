using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TitanBot.Core.Models;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services;
using TitanBot.Core.Services.Command;
using TitanBot.Core.Services.Database;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Logging;
using TitanBot.Core.Services.Messaging;
using TitanBot.Core.Services.TypeReader;
using TitanBot.Models;
using TitanBot.Models.Contexts;
using TitanBot.Services;
using TitanBot.Services.Command.Models;
using TitanBot.Services.Database;
using TitanBot.Services.Formatting;
using TitanBot.Services.Logging;
using TitanBot.Services.Messaging;
using TitanBot.Services.TypeReader;

namespace TitanBot.Console
{
    internal class Program
    {
        private static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            var factory = new ServiceFactory(services =>
            {
                services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Debug,
                    MessageCacheSize = 1000
                }))
                .AddSingleton<IStartupService, StartupService>()
                .AddSingleton<ILoggerService, LoggerService>()
                .AddSingleton<IMessageService, MessageService>()
                .AddSingleton<ITranslationService, TranslationService>()
                .AddSingleton<IDatabaseService, DatabaseService>()
                .AddSingleton<IFormatterService, FormatterService>()
                .AddSingleton<IConfiguration>(TBConfig.Load())
                .AddSingleton<ICommandService, CommandService>()
                .AddTransient<ICommandContext, CommandContext>()
                .AddSingleton<ITypeReaderService, TypeReaderService>()
                .AddSingleton(new Random());
            });

            factory.GetRequiredService<ILoggerService>();
            factory.GetRequiredService<IMessageService>();
            factory.GetRequiredService<ICommandService>();
            var startup = factory.GetRequiredService<IStartupService>();

            var typeReader = factory.GetRequiredService<ITypeReaderService>();

            var res = new[]
            {
                typeReader.Read(null, "0", typeof(int)),
                typeReader.Read(null, "2", typeof(int)),
                typeReader.Read(null, "a", typeof(int)),
                typeReader.Read(null, "1.1", typeof(int)),
                typeReader.Read(null, "1,2,3,4", typeof(int[])),
                typeReader.Read(null, "Critical", typeof(LogSeverity)),
                typeReader.Read(null, "0", typeof(LogSeverity)),
                typeReader.Read(null, "1", typeof(LogSeverity)),
                typeReader.Read(null, "apskado", typeof(LogSeverity)),
                typeReader.Read(null, "1,Error,Critical,3", typeof(LogSeverity[])),
            };

            await startup.StartAsync();
            await startup.WhileRunning;
        }
    }
}