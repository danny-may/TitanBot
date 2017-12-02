using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Titanbot.Commands;
using Titanbot.Config;
using Titanbot.Extensions;
using Titanbot.Startup.Interfaces;
using Titansmasher.Services.Database.Interfaces;
using Titansmasher.Services.Logging;
using Titansmasher.Services.Logging.Interfaces;

namespace Titanbot.LiveTesting
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var program = new Program();
            var services = new ServiceCollection();
            program.Configure(services);
            var provider = services.BuildServiceProvider();
            program.RunAsync(provider.GetRequiredService<IStartup>())
                   .GetAwaiter()
                   .GetResult();
        }

        private IConfigurationRoot Configuration { get; }

        public Program()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appSettings.json");
            Configuration = builder.Build();
        }

        public void Configure(IServiceCollection services)
        {
            //Temproary fix for a bug in .netcore 2.0 https://github.com/dotnet/project-system/issues/2239
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            services.AddOptions();

            services.AddTitanbot()
                    .AddSingleton<IStartup, TitanbotController>()
                    .AddSingleton<ILoggerService, LoggerService>();

            services.Configure<DiscordSocketConfig>(Configuration.GetSection("Discord"))
                    .Configure<DatabaseConfig>(Configuration.GetSection("Database"))
                    .Configure<BotConfig>(Configuration.GetSection("Bot"))
                    .Configure<CommandConfig>(Configuration.GetSection("Commands"));

            services.AddSingleton<Random>();
        }

        public async Task RunAsync(IStartup startup)
        {
            await startup.StartAsync();
            await startup.WhileConnected;
        }
    }
}