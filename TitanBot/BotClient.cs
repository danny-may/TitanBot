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
using TitanBot.Commands;
using TitanBot.Contexts;
using TitanBot.Dependencies;
using TitanBot.DiscordHandlers;
using TitanBot.Downloader;
using TitanBot.Formatting;
using TitanBot.Logging;
using TitanBot.Replying;
using TitanBot.Scheduling;
using TitanBot.Settings;
using TitanBot.Storage;
using TitanBot.TypeReaders;
using static TitanBot.TBLocalisation.Settings;

namespace TitanBot
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
        public ISettingManager SettingsManager { get; private set; }
        public ITextResourceManager TextResourceManager { get; private set; }
        private ISettingContext GlobalSettings => SettingsManager.GetContext(SettingsManager.Global);
        private GeneralGlobalSetting GeneralGlobalSettings => GlobalSettings.Get<GeneralGlobalSetting>();
        private Lazy<IUser> Owner { get; }
        public IReadOnlyList<ulong> Owners => GeneralGlobalSettings.Owners.Concat(new ulong[] { Owner.Value.Id })
                                                                   .ToList().AsReadOnly();
        public Task UntilOffline => Task.Run(async () => { while (DiscordClient.LoginState != LoginState.LoggedOut) { await Task.Delay(10); } });
        private List<DiscordHandlerBase> Handlers { get; } = new List<DiscordHandlerBase>();
        public Type[] DefaultCommands => Assembly.GetAssembly(GetType()).GetTypes().Where(t => t.IsSubclassOf(typeof(Command))).ToArray();

        private ManualResetEvent readyEvent = new ManualResetEvent(false);

        public BotClient() : this(null, null) { }

        public BotClient(Action<IDependencyFactory> mapper) : this(null, mapper) { }

        public BotClient(IDependencyFactory factory, Action<IDependencyFactory> mapper)
        {
            Owner = new Lazy<IUser>(() => DiscordClient.GetApplicationInfoAsync().Result.Owner);
            mapper = mapper ?? (f => { });
            DependencyFactory = factory ?? new DependencyFactory();
            MapDefaults();
            mapper(DependencyFactory);

            Logger = DependencyFactory.GetOrStore<ILogger>();
            DiscordClient = DependencyFactory.GetOrStore<DiscordSocketClient>();
            TypeReaders = DependencyFactory.GetOrStore<ITypeReaderCollection>();
            Database = DependencyFactory.GetOrStore<IDatabase>();
            SettingsManager = DependencyFactory.GetOrStore<ISettingManager>();
            Scheduler = DependencyFactory.GetOrStore<IScheduler>();
            CommandService = DependencyFactory.GetOrStore<ICommandService>();
            DependencyFactory.GetOrStore<IDownloader>();
            DependencyFactory.GetOrStore<ValueFormatter>();
            DependencyFactory.GetOrStore<IReplier>();
            DependencyFactory.GetOrStore<IPermissionManager>();
            TextResourceManager = DependencyFactory.GetOrStore<ITextResourceManager>();

            SetupFeatures();
            InstallSettingEditors();
            SubscribeEvents();

            InstallHandlers(Assembly.GetExecutingAssembly());
        }

        private void SetupFeatures()
        {
            TextResourceManager.RegisterKeys(TBLocalisation.Defaults);
        }

        private void InstallSettingEditors()
        {
            SettingsManager.GetEditorCollection<GeneralGlobalSetting>(SettingScope.Global)
                           .WithName("General")
                           .WithDescription(Desc.GLOBAL_GENERAL)
                           .AddSetting(s => s.DefaultPrefix, s => s.SetAlias("Prefix", "Pfx"))
                           .AddSetting<IUser>(s => s.Owners);
            SettingsManager.GetEditorCollection<GeneralGuildSetting>(SettingScope.Guild)
                           .WithName("General")
                           .WithDescription(Desc.GUILD_GENERAL)
                           .AddSetting(s => s.Prefix, s => s.SetAlias("Prefix", "Pfx"))
                           .AddSetting(s => s.PermOverride)
                           .AddSetting<IRole>(s => s.RoleOverride)
                           .AddSetting(s => s.DateTimeFormat)
                           .AddSetting(s => s.PreferredLanguage, (Locale a) => (string)a, s => s.SetAlias("Lang", "Locale"))
                           .WithNotes(Notes.GUILD_GENERAL);
            SettingsManager.GetEditorCollection<GeneralUserSetting>(SettingScope.User)
                           .WithName("General")
                           .WithDescription(Desc.USER_GENERAL)
                           .AddSetting(s => s.Language, (Locale a) => (string)a)
                           .AddSetting(s => s.FormatType, (FormatType a) => (uint)a, b => b.SetViewer((c, f) => (FormatType)f))
                           .AddSetting(s => s.UseEmbeds, s => s.SetAlias("Embed", "Embeds", "UseEmbed"));
        }

        private void MapDefaults()
        {
            DependencyFactory.Store(this);
            DependencyFactory.TryMap<ILogger, Logger>();
            DependencyFactory.TryMap<IDatabase, Database>();
            DependencyFactory.TryMap<IScheduler, Scheduler>();
            DependencyFactory.TryMap<ICommandService, CommandService>();
            DependencyFactory.TryMap<IReplier, Replier>();
            DependencyFactory.TryMap<Contexts.ICommandContext, CommandContext>();
            DependencyFactory.TryMap<IPermissionManager, PermissionManager>();
            DependencyFactory.TryMap<ISettingManager, SettingManager>();
            DependencyFactory.TryMap(typeof(ISettingEditorCollection<>), typeof(SettingEditorCollection<>));
            DependencyFactory.TryMap<IDownloader, CachedDownloader>();
            DependencyFactory.TryMap<ValueFormatter, ValueFormatter>();
            DependencyFactory.TryMap<ITextResourceManager, TextResourceManager>();
            DependencyFactory.TryMap<ITypeReaderCollection, TypeReaderCollection>();
        }

        public void InstallHandlers(Assembly assembly)
        {
            var handlers = assembly.GetTypes()
                                   .Where(t => t.IsSubclassOf(typeof(DiscordHandlerBase)));
            foreach (var handler in handlers)
            {
                if (DependencyFactory.TryConstruct(handler, out object obj))
                    Handlers.Add((DiscordHandlerBase)obj);
            }
        }

        private void SubscribeEvents()
        {
            DiscordClient.Ready += () => { readyEvent.Set(); return Task.CompletedTask; };
            DiscordClient.Log += m => { Logger.Log(DiscordUtil.ToLoggable(m)); return Task.CompletedTask; };
        }

        public Task StartAsync()
            => StartAsync(x => GeneralGlobalSettings.Token);
        public Task StartAsync(string token)
            => StartAsync(x => token);
        public async Task StartAsync(Func<string, string> tokenInput)
        {
            var token = tokenInput(GeneralGlobalSettings.Token) ?? GeneralGlobalSettings.Token;
            GlobalSettings.Edit<GeneralGlobalSetting>(s => { if (!string.IsNullOrWhiteSpace(token)) { s.Token = token; } });
            if (DiscordClient.LoginState != LoginState.LoggedOut)
                return;
            try
            {
                await DiscordClient.LoginAsync(TokenType.Bot, GeneralGlobalSettings.Token);
            }
            catch (HttpException ex)
            {
                if (ex.HttpCode == HttpStatusCode.Unauthorized)
                    GlobalSettings.Edit<GeneralGlobalSetting>(s => s.Token = null);
                throw;
            }
            await DiscordClient.StartAsync();
            readyEvent.WaitOne();
            readyEvent.Reset();

            Scheduler.Enabled = true;
        }

        public async Task StopAsync()
        {
            Scheduler.Enabled = false;
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
