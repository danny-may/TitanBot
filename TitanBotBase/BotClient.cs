using Discord;
using Discord.WebSocket;
using TitanBotBase.Database;
using TitanBotBase.Dependencies;
using TitanBotBase.Logger;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Scheduler;
using System.Threading;
using System.Reflection;
using TitanBotBase.DiscordHandlers;
using TitanBotBase.Util;
using TitanBotBase.Commands;
using TitanBotBase.TypeReaders;
using TitanBotBase.Formatter;
using TitanBotBase.Settings;

namespace TitanBotBase
{
    public class BotClient : IDisposable
    {
        public DiscordSocketClient DiscordClient { get; private set; }
        public ILogger Logger { get; private set; }
        public IDatabase Database { get; private set; }
        public IDependencyFactory DependencyFactory { get; private set; }
        public IScheduler Scheduler { get; private set; }
        public ICommandService CommandService { get; private set; }
        public ITypeReaderCollection TypeReaders { get; private set; }
        public ISettingsManager SettingsManager { get; private set; }
        public GlobalSetting GlobalSettings => SettingsManager.GetGlobalSettings();
        public IReadOnlyList<ulong> Owners => GlobalSettings.Owners.Concat(new ulong[] { DiscordClient.GetApplicationInfoAsync().Result.Owner.Id })
                                                                   .ToList().AsReadOnly();
        private List<DiscordHandlerBase> Handlers { get; } = new List<DiscordHandlerBase>();

        private ManualResetEvent readyEvent = new ManualResetEvent(false);

        public BotClient() : this(null, null) { }

        public BotClient(Action<IDependencyFactory> mapper) : this(null, mapper) { }

        public BotClient(IDependencyFactory factory, Action<IDependencyFactory> mapper)
        { 
            mapper = mapper ?? (f => { });
            DependencyFactory = factory ?? new DependencyFactory();

            DependencyFactory.Store(this);
            DependencyFactory.TryMap<ILogger, TitanBotLogger>();
            DependencyFactory.TryMap<IDatabase, TitanBotDb>();
            DependencyFactory.TryMap<IScheduler, TitanBotScheduler>();
            DependencyFactory.TryMap<ICommandService, CommandService>();
            DependencyFactory.TryMap<IReplier, Replier>();
            DependencyFactory.TryMap<ICommandContext, CommandContext>();
            DependencyFactory.TryMap<ITypeReaderCollection, TypeReaderCollection>();
            DependencyFactory.TryMap<IPermissionChecker, PermissionChecker>();
            DependencyFactory.TryMap<OutputFormatter, BaseFormatter>();
            DependencyFactory.TryMap<ISettingsManager, SettingsManager>();
            mapper(DependencyFactory);

            Logger = DependencyFactory.ConstructAndStore<ILogger>();
            DiscordClient = DependencyFactory.ConstructAndStore<DiscordSocketClient>();
            TypeReaders = DependencyFactory.ConstructAndStore<ITypeReaderCollection>();
            Database = DependencyFactory.ConstructAndStore<IDatabase>();
            Scheduler = DependencyFactory.ConstructAndStore<IScheduler>();
            CommandService = DependencyFactory.ConstructAndStore<ICommandService>();
            SettingsManager = DependencyFactory.ConstructAndStore<ISettingsManager>();

            SubscribeEvents();

            Install(Assembly.GetExecutingAssembly());
        }

        public void Install(Assembly assembly)
        {
            var handlers = assembly.GetTypes()
                                   .Where(t => t.IsSubclassOf(typeof(DiscordHandlerBase)));
            foreach (var handler in handlers)
            {
                if (DependencyFactory.TryConstruct(handler, out object obj))
                    Handlers.Add((DiscordHandlerBase)obj);
            }
            CommandService.Install(assembly);
        }

        private void SubscribeEvents()
        {
            DiscordClient.Ready += () => Task.Run(() => readyEvent.Set());
            DiscordClient.Log += m => Logger.LogAsync(DiscordUtil.ToLoggable(m));
        }

        public async Task StartAsync(string token)
        {
            if (DiscordClient.LoginState != LoginState.LoggedOut)
                return;
            await DiscordClient.LoginAsync(TokenType.Bot, token);
            await DiscordClient.StartAsync();
            readyEvent.WaitOne();
            readyEvent.Reset();

            await Scheduler.StartAsync();
        }

        public async Task StopAsync()
        {
            await Scheduler.StopAsync();
            await DiscordClient.LogoutAsync();
        }

        public void Dispose()
        {
            Database?.Dispose();
            DiscordClient?.Dispose();
            DependencyFactory?.Dispose();
        }
    }
}
