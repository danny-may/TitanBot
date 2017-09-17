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
using TitanBot.Models;
using TitanBot.Models.Contexts;
using TitanBot.Services;
using TitanBot.Services.Command.Models;
using TitanBot.Services.Database;
using TitanBot.Services.Formatting;
using TitanBot.Services.Logging;
using TitanBot.Services.Messaging;

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
                .AddSingleton<ICommandContext, CommandContext>()
                .AddSingleton(new Random());
            });

            factory.GetRequiredService<ILoggerService>();
            factory.GetRequiredService<IMessageService>();
            factory.GetRequiredService<ICommandService>();
            var startup = factory.GetRequiredService<IStartupService>();

            await startup.StartAsync();
            await startup.WhileRunning;
        }
    }
}