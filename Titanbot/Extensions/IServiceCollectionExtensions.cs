using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Titanbot.Commands;
using Titanbot.Commands.Interfaces;
using Titanbot.Permissions;
using Titanbot.Permissions.Interfaces;
using Titanbot.Settings;
using Titanbot.Settings.Interfaces;
using Titansmasher.Services.Database.Interfaces;
using Titansmasher.Services.Database.LiteDb;
using Titansmasher.Services.Display;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddTitanbot(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDiscord()
                             .AddLiteDb()
                             .AddDisplayService()
                             .AddSettingService()
                             .AddCommandService();

            return serviceCollection;
        }

        public static IServiceCollection AddDiscord(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(p => new DiscordSocketClient(p.GetService<IOptions<DiscordSocketConfig>>()?.Value ?? new DiscordSocketConfig()));

            return serviceCollection;
        }

        public static IServiceCollection AddCommandService(this IServiceCollection serviceCollection)
            => serviceCollection.AddCommandService<CommandService>();

        public static IServiceCollection AddCommandService<TService>(this IServiceCollection serviceCollection)
            where TService : class, ICommandService
        {
            serviceCollection.AddSingleton(serviceCollection)
                             .AddSingleton<ICommandService, TService>()
                             .AddSingleton<IMessageSplitter, RegexSplitter>()
                             .AddSingleton<IPermissionManager, PermissionManager>();

            return serviceCollection;
        }

        public static IServiceCollection AddLiteDb(this IServiceCollection serviceCollection)
            => serviceCollection.AddDatabase<LiteDbService>();

        public static IServiceCollection AddDatabase<TService>(this IServiceCollection serviceCollection)
            where TService : class, IDatabaseService
        {
            serviceCollection.AddSingleton<IDatabaseService, TService>();
            return serviceCollection;
        }

        public static IServiceCollection AddDisplayService(this IServiceCollection serviceCollection)
            => serviceCollection.AddDisplayService<DisplayService>();

        public static IServiceCollection AddDisplayService<TService>(this IServiceCollection serviceCollection)
            where TService : class, IDisplayService
        {
            serviceCollection.AddSingleton<IDisplayService, TService>();
            return serviceCollection;
        }

        public static IServiceCollection AddSettingService(this IServiceCollection serviceCollection)
            => serviceCollection.AddSettingService<SettingService>();

        public static IServiceCollection AddSettingService<TService>(this IServiceCollection serviceCollection)
            where TService : class, ISettingService
        {
            serviceCollection.AddSingleton<ISettingService, TService>();
            return serviceCollection;
        }
    }
}