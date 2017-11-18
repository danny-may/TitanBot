using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Titanbot.Commands;
using Titanbot.Commands.Interfaces;
using Titanbot.Config;
using Titanbot.Startup.Interfaces;
using Titansmasher.Services.Configuration.Extensions;
using Titansmasher.Services.Database.Interfaces;
using Titansmasher.Services.Database.LiteDb;
using Titansmasher.Services.Logging;
using Titansmasher.Services.Logging.Interfaces;

namespace Titanbot.LiveTesting
{
    internal class Program
    {
        private static void Main(string[] args)
            => new Program().RunAsync().GetAwaiter().GetResult();

        public async Task RunAsync()
        {
            //Temproary fix for a bug in .netcore 2.0 https://github.com/dotnet/project-system/issues/2239
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            Titanbot.BaseCommands.ForceLoad.Load();

            var provider = BuildServiceProvider();

            await StartBot(provider);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingletonConfigService()
                    .AddSingleton(p => p.GetRequiredService<BotConfig>().SocketConfig())
                    .AddSingletonConfig<BotConfig>()
                    .AddSingletonConfig<LiteDbConfig>()
                    .AddSingleton<DiscordSocketClient>()
                    .AddSingleton<ILoggerService, LoggerService>()
                    .AddSingleton<IDatabaseService, LiteDbService>()
                    .AddSingleton<ICommandService, CommandService>()
                    .AddSingleton<IStartup, TitanbotController>()
                    .AddSingleton<Random>();

            return services.BuildServiceProvider();
        }

        private async Task StartBot(IServiceProvider provider)
        {
            var startup = provider.GetRequiredService<IStartup>();

            await startup.StartAsync();

            await startup.WhileConnected;
        }
    }
}