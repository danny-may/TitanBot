using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Core.Models;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services;
using TitanBot.Core.Services.Command;
using TitanBot.Core.Services.Database;
using TitanBot.Core.Services.Dependency;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Logging;
using TitanBot.Core.Services.Messaging;
using TitanBot.Core.Services.TypeReader;
using TitanBot.Models;
using TitanBot.Models.Contexts;
using TitanBot.Services;
using TitanBot.Services.Command;
using TitanBot.Services.Database;
using TitanBot.Services.Dependency;
using TitanBot.Services.Formatting;
using TitanBot.Services.Logging;
using TitanBot.Services.Messaging;
using TitanBot.Services.TypeReader;

namespace TitanBot
{
    public class DefaultTitanBotClient
    {
        public static IInstanceProvider GetDefaultInjector()
            => new InstanceProvider()
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
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
            .AddTransient<IConfiguration>(i => TBConfig.Load())
            .AddSingleton<ICommandService, CommandService>()
            .AddTransient<ICommandContext, CommandContext>()
            .AddSingleton<ITypeReaderService, TypeReaderService>()
            .AddSingleton(new Random());

        public List<Type> RequiredInstances { get; } = new List<Type>
        {
            typeof(ICommandService)
        };

        public Task WhileRunning => _startup.WhileRunning;

        private IInstanceProvider _factory;
        private IStartupService _startup => _factory.GetRequiredInstance<IStartupService>();

        public DefaultTitanBotClient()
        {
            _factory = GetDefaultInjector();
        }

        public async Task StartAsync()
        {
            foreach (var type in RequiredInstances)
                _factory.GetRequiredInstance(type);

            await _startup.StartAsync();
        }

        public async Task StopAsync()
            => await _startup.StopAsync();
    }
}