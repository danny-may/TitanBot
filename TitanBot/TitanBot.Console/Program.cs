using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using TitanBot.Core.Models;
using TitanBot.Core.Services;
using TitanBot.Core.Services.Logging;
using TitanBot.Models;
using TitanBot.Services;
using TitanBot.Services.Logging;

namespace TitanBot.Console
{
    internal class Program
    {
        private static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            var config = TBConfig.Load();

            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Debug,
                    MessageCacheSize = 1000
                }))
                .AddSingleton<IStartupService, StartupService>()
                .AddSingleton<ILoggerService, LoggerService>()
                .AddSingleton<IConfiguration>(config);

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<ILoggerService>();
            await provider.GetRequiredService<IStartupService>().StartAsync();

            await Task.Delay(-1);
        }
    }
}