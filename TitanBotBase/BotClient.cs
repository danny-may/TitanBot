using Discord;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Database;
using TitanBotBase.Dependencies;
using TitanBotBase.DiscordHandlers;
using TitanBotBase.Downloader;
using TitanBotBase.Formatter;
using TitanBotBase.Logger;
using TitanBotBase.Scheduler;
using TitanBotBase.Settings;
using TitanBotBase.TypeReaders;
using TitanBotBase.Util;

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
        public GlobalSetting GlobalSettings => SettingsManager.GlobalSettings;
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
            
            MapDefaults();
            mapper(DependencyFactory);

            Logger = DependencyFactory.ConstructAndStore<ILogger>();
            DiscordClient = DependencyFactory.ConstructAndStore<DiscordSocketClient>();
            TypeReaders = DependencyFactory.ConstructAndStore<ITypeReaderCollection>();
            Database = DependencyFactory.ConstructAndStore<IDatabase>();
            SettingsManager = DependencyFactory.ConstructAndStore<ISettingsManager>();
            Scheduler = DependencyFactory.ConstructAndStore<IScheduler>();
            CommandService = DependencyFactory.ConstructAndStore<ICommandService>();
            DependencyFactory.ConstructAndStore<IDownloader>();

            SubscribeEvents();

            Install(Assembly.GetExecutingAssembly());
        }

        public void MapDefaults()
        {
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
            DependencyFactory.TryMap<IDownloader, CachedDownloader>();
            DependencyFactory.TryMap<IEditableSettingGroup, EditableSettingGroup>();
            DependencyFactory.TryMap(typeof(IEditableSettingBuilder<>), typeof(EditableSettingBuilder<>));
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

        public Task StartAsync()
            => StartAsync(x => GlobalSettings.Token);
        public Task StartAsync(string token)
            => StartAsync(x => token);
        public async Task StartAsync(Func<string, string> tokenInput)
        {
            var token = tokenInput(GlobalSettings.Token) ?? GlobalSettings.Token;
            token = string.IsNullOrWhiteSpace(token) ? GlobalSettings.Token : token;
            GlobalSettings.Token = token;
            if (DiscordClient.LoginState != LoginState.LoggedOut)
                return;
            try
            {
                await DiscordClient.LoginAsync(TokenType.Bot, token);
            }
            catch (HttpException ex)
            {
                if (ex.HttpCode == HttpStatusCode.Unauthorized)
                    GlobalSettings.Token = null;
                throw;
            }
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
