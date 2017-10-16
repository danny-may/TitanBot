using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Contexts;
using TitanBot.Dependencies;
using TitanBot.Downloader;
using TitanBot.Formatting;
using TitanBot.Formatting.Interfaces;
using TitanBot.Logging;
using TitanBot.Replying;
using TitanBot.Scheduling;
using TitanBot.Settings;
using TitanBot.Storage;
using static TitanBot.TBLocalisation.Logic;

namespace TitanBot.Commands
{
    public abstract class Command
    {
        //locks
        private static ConcurrentDictionary<Type, object> CommandLocks { get; } = new ConcurrentDictionary<Type, object>();
        private static ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>> GuildLocks { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>>();
        private static ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>> ChannelLocks { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>>();
        protected internal static bool DisablePings;

        protected object GlobalCommandLock => CommandLocks.GetOrAdd(GetType(), new object());
        protected object GuildCommandLock => GuildLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong?, object>()).GetOrAdd(Context.Guild?.Id, new object());
        protected object ChannelCommandLock => ChannelLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong, object>()).GetOrAdd(Context.Channel.Id, new object());
        protected object InstanceCommandLock { get; } = new object();

        public static int TotalCommands { get; private set; } = 0;

        private bool HasReplied { get; set; }
        private IUserMessage AwaitMessage { get; set; }

        protected virtual LocalisedString DelayMessage => (LocalisedString)COMMAND_DELAY_DEFAULT;
        protected virtual int DelayMessageMs => 3000;

        protected IUserMessage Message => Context?.Message;
        protected DiscordSocketClient Client => Context?.Client;
        protected IUser Author => Context?.Author;
        protected IMessageChannel Channel => Context?.Channel;
        protected SocketSelfUser BotUser => Client?.CurrentUser;
        protected IGuild Guild => Context?.Guild;

        protected ValueFormatter Formatter => Context?.Formatter;
        private IReplier Replier => Context?.Replier;
        protected ITextResourceCollection TextResource => Context?.TextResource;
        protected string Prefix => Context.Prefix;
        protected string CommandName => Context.CommandText;
        protected ISettingContext GlobalSettings => SettingsManager.GetContext(SettingsManager.Global);
        protected ISettingContext GuildSettings => Guild == null ? null : SettingsManager.GetContext(Guild);
        protected ISettingContext UserSettings => SettingsManager.GetContext(Author);
        protected ISettingContext ChannelSettings => SettingsManager.GetContext(Channel);
        protected GeneralGlobalSetting GeneralGlobalSetting => GlobalSettings.Get<GeneralGlobalSetting>();
        protected GeneralGuildSetting GeneralGuildSetting => GuildSettings?.Get<GeneralGuildSetting>();
        protected GeneralUserSetting GeneralUserSetting => UserSettings.Get<GeneralUserSetting>();
        protected IGuildUser GuildBotUser => Guild?.GetCurrentUserAsync().Result;
        protected IGuildUser GuildAuthor => Author as IGuildUser;
        protected IGuildChannel GuildChannel => Channel as IGuildChannel;
        protected string[] AcceptedPrefixes => new string[] { BotUser?.Mention, BotUser?.Username, GeneralGlobalSetting.DefaultPrefix, GeneralGuildSetting?.Prefix }
                                                    .Where(p => !string.IsNullOrWhiteSpace(p))
                                                    .ToArray();

        protected ICommandContext Context { get; set; }
        protected BotClient Bot { get; set; }
        protected ILogger Logger { get; private set; }
        protected ICommandService CommandService { get; private set; }
        protected ISettingManager SettingsManager { get; private set; }
        protected IDatabase Database { get; private set; }
        protected IScheduler Scheduler { get; private set; }
        protected IDownloader Downloader { get; private set; }

        public Command()
        {
            TotalCommands++;
        }

        internal void Install(ICommandContext context, IDependencyFactory factory)
        {
            Context = context;

            if (Replier is Replier r)
                r.DisablePings = DisablePings;

            Logger = factory.Get<ILogger>();
            Database = factory.Get<IDatabase>();
            Bot = factory.Get<BotClient>();
            CommandService = factory.Get<ICommandService>();
            Scheduler = factory.Get<IScheduler>();
            SettingsManager = factory.Get<ISettingManager>();
            Downloader = factory.Get<IDownloader>();

            StartReplyTimer();
        }

        private void StartReplyTimer()
        {
            Task.Run(async () =>
            {
                await Task.Delay(DelayMessageMs);
                lock (InstanceCommandLock)
                {
                    if (!HasReplied)
                    {
                        AwaitMessage = Replier.Reply(Channel, Author)
                                              .WithMessage(DelayMessage)
                                              .SendAsync(true).Result;
                    }
                }
            }).DontWait();
        }

        private Task OnMessageSent(IReplyContext context, IUserMessage message)
        {
            lock (InstanceCommandLock)
            {
                HasReplied = true;
                if (AwaitMessage != null)
                {
                    AwaitMessage.DeleteAsync().Wait();
                    AwaitMessage = null;
                }
            }
            return Task.CompletedTask;
        }

        protected IReplyContext Reply()
            => Reply(Channel, Author);
        protected IReplyContext Reply(IMessageChannel channel)
            => Reply(channel, Author);
        protected IReplyContext Reply(IUser user)
            => Reply(Channel, user);
        protected IReplyContext Reply(IMessageChannel channel, IUser user)
        {
            var rContext = Replier.Reply(channel, user);
            rContext.OnSend += OnMessageSent;
            return rContext;
        }

        protected IModifyContext Modify(IUserMessage message)
            => Modify(message, Author);
        protected IModifyContext Modify(IUserMessage message, IUser user)
            => Replier.Modify(message, user);

        protected ValueTask<IUserMessage> ReplyAsync(ILocalisable<string> message)
            => Reply().WithMessage(message).SendAsync();
        protected ValueTask<IUserMessage> ReplyAsync(IEmbedable embedable)
            => Reply().WithEmbedable(embedable).SendAsync();
        protected ValueTask<IUserMessage> ReplyAsync(ILocalisable<string> message, IEmbedable embedable)
            => Reply().WithMessage(message).WithEmbedable(embedable).SendAsync();

        protected ValueTask<IUserMessage> ReplyAsync(string key)
            => ReplyAsync(new LocalisedString(key));
        protected ValueTask<IUserMessage> ReplyAsync(string key, ReplyType replyType)
            => ReplyAsync(new LocalisedString(key, replyType));
        protected ValueTask<IUserMessage> ReplyAsync(string key, params object[] values)
            => ReplyAsync(new LocalisedString(key, values));
        protected ValueTask<IUserMessage> ReplyAsync(string key, ReplyType replyType, params object[] values)
            => ReplyAsync(new LocalisedString(key, replyType, values));

        protected ValueTask<IUserMessage> ReplyAsync(Embedable embedable)
            => ReplyAsync((IEmbedable)embedable);
        protected ValueTask<IUserMessage> ReplyAsync(ILocalisable<EmbedBuilder> embedable)
            => ReplyAsync(Embedable.FromEmbed(embedable));
        protected ValueTask<IUserMessage> ReplyAsync(LocalisedEmbedBuilder embedable)
            => ReplyAsync((ILocalisable<EmbedBuilder>)embedable);

        protected ValueTask<IUserMessage> ReplyAsync(string key, Embedable embedable)
            => ReplyAsync(new LocalisedString(key), embedable);
        protected ValueTask<IUserMessage> ReplyAsync(string key, ReplyType replyType, Embedable embedable)
            => ReplyAsync(new LocalisedString(key, replyType), embedable);
        protected ValueTask<IUserMessage> ReplyAsync(string key, Embedable embedable, params object[] values)
            => ReplyAsync(new LocalisedString(key, values), embedable);
        protected ValueTask<IUserMessage> ReplyAsync(string key, ReplyType replyType, Embedable embedable, params object[] values)
            => ReplyAsync(new LocalisedString(key, replyType, values), embedable);

        protected ValueTask<IUserMessage> ReplyAsync(string key, ILocalisable<EmbedBuilder> embedable)
            => ReplyAsync(key, Embedable.FromEmbed(embedable));
        protected ValueTask<IUserMessage> ReplyAsync(string key, ReplyType replyType, ILocalisable<EmbedBuilder> embedable)
            => ReplyAsync(key, replyType, Embedable.FromEmbed(embedable));
        protected ValueTask<IUserMessage> ReplyAsync(string key, ILocalisable<EmbedBuilder> embedable, params object[] values)
            => ReplyAsync(key, Embedable.FromEmbed(embedable), values);
        protected ValueTask<IUserMessage> ReplyAsync(string key, ReplyType replyType, ILocalisable<EmbedBuilder> embedable, params object[] values)
            => ReplyAsync(key, replyType, Embedable.FromEmbed(embedable), values);

        protected ValueTask<IUserMessage> ReplyAsync(string key, LocalisedEmbedBuilder embedable)
            => ReplyAsync(key, (ILocalisable<EmbedBuilder>)embedable);
        protected ValueTask<IUserMessage> ReplyAsync(string key, ReplyType replyType, LocalisedEmbedBuilder embedable)
            => ReplyAsync(key, replyType, (ILocalisable<EmbedBuilder>)embedable);
        protected ValueTask<IUserMessage> ReplyAsync(string key, LocalisedEmbedBuilder embedable, params object[] values)
            => ReplyAsync(key, (ILocalisable<EmbedBuilder>)embedable, values);
        protected ValueTask<IUserMessage> ReplyAsync(string key, ReplyType replyType, LocalisedEmbedBuilder embedable, params object[] values)
            => ReplyAsync(key, replyType, (ILocalisable<EmbedBuilder>)embedable, values);

        protected string Beautify<T>(T value)
            => Formatter.Beautify(GeneralUserSetting.FormatType, value);
        protected string Beautify(object value)
             => Formatter.Beautify(GeneralUserSetting.FormatType, value);
        protected string Beautify(Type type, object value)
             => Formatter.Beautify(GeneralUserSetting.FormatType, type, value);
    }
}