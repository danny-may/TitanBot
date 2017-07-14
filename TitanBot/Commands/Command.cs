using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Dependencies;
using TitanBot.Downloader;
using TitanBot.Formatting;
using TitanBot.Logging;
using TitanBot.Scheduling;
using TitanBot.Settings;
using TitanBot.Storage;
using TitanBot.Util;

namespace TitanBot.Commands
{
    public abstract class Command
    {
        static ConcurrentDictionary<Type, object> CommandLocks { get; } = new ConcurrentDictionary<Type, object>();
        static ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>> GuildLocks { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>>();
        static ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>> ChannelLocks { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>>();
        bool HasReplied { get; set; }
        IUserMessage AwaitMessage { get; set; }
        ICommandContext Context { get; set; }

        protected virtual string DelayMessage => "COMMAND_DELAY_DEFAULT";
        protected virtual int DelayMessageMs => 3000;

        protected BotClient Bot { get; set; }
        protected ILogger Logger { get; private set; }
        protected ICommandService CommandService { get; private set; }
        protected IUserMessage Message => Context?.Message;
        protected DiscordSocketClient Client => Context?.Client;
        protected IUser Author => Context?.Author;
        protected IMessageChannel Channel => Context?.Channel;
        protected SocketSelfUser BotUser => Client?.CurrentUser;
        protected IGuild Guild => Context?.Guild;
        protected IGuildUser GuildAuthor => Author as IGuildUser;
        protected IGuildChannel GuildChannel => Channel as IGuildChannel;
        protected IGuildUser GuildBotUser => Guild?.GetCurrentUserAsync().Result;
        protected ISettingsManager SettingsManager { get; private set; }
        protected GlobalSetting GlobalSettings => SettingsManager?.GlobalSettings;
        protected GeneralSettings GuildData { get; private set; }
        protected IDatabase Database { get; private set; }
        protected IScheduler Scheduler { get; private set; }
        protected IReplier Replier { get; private set; }
        protected IDownloader Downloader { get; private set; }
        protected ValueFormatter Formatter { get; private set; }
        protected ITextResourceCollection TextResource { get; private set; }
        protected string[] AcceptedPrefixes => new string[] { BotUser.Mention, BotUser.Username, SettingsManager.GlobalSettings.DefaultPrefix, GuildData?.Prefix }.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
        protected object GlobalCommandLock => CommandLocks.GetOrAdd(GetType(), new object());
        protected object GuildCommandLock => GuildLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong?, object>()).GetOrAdd(Context.Guild?.Id, new object());
        protected object ChannelCommandLock => ChannelLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong, object>()).GetOrAdd(Context.Channel.Id, new object());
        protected object InstanceCommandLock { get; } = new object();

        protected string Prefix { get; private set; }
        protected string CommandName { get; private set; }

        public static int TotalCommands { get; private set; } = 0;

        public Command()
        {
            TotalCommands++;
        }

        internal void Install(ICommandContext context, IDependencyFactory factory)
        {
            Context = context;
            Replier = Context.Replier;
            TextResource = Context.TextResource;
            Formatter = Context.Formatter;
            Logger = factory.Get<ILogger>();
            Database = factory.Get<IDatabase>();
            Bot = factory.Get<BotClient>();
            CommandService = factory.Get<ICommandService>();
            Scheduler = factory.Get<IScheduler>();
            SettingsManager = factory.Get<ISettingsManager>();
            Downloader = factory.Get<IDownloader>();
            if (Guild != null)
                GuildData = SettingsManager.GetGroup<GeneralSettings>(Guild.Id);
            Prefix = context.Prefix;
            CommandName = context.CommandText;
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
                                              .Send();
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

        protected IReplyContext Reply
        {
            get
            {
                var context = Replier.Reply(Channel, Author);
                context.OnSend += OnMessageSent;
                return context;
            }
        }

        protected Task<IUserMessage> ReplyAsync(string message)
            => Reply.WithMessage(message).SendAsync();
        protected Task<IUserMessage> ReplyAsync(string message, ReplyType replyType)
            => Reply.WithMessage(message).SendAsync();
        protected Task<IUserMessage> ReplyAsync(string message, params object[] values)
            => Reply.WithMessage(message, values).SendAsync();
        protected Task<IUserMessage> ReplyAsync(string message, ReplyType replyType, params object[] values)
            => Reply.WithMessage(message, replyType, values).SendAsync();
        protected Task<IUserMessage> ReplyAsync(Embed embed)
            => Reply.WithEmbed(embed).SendAsync();
    }
}
