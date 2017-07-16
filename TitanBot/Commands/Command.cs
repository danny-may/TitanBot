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
    public abstract partial class Command
    {
        //locks
        static ConcurrentDictionary<Type, object> CommandLocks { get; } = new ConcurrentDictionary<Type, object>();
        static ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>> GuildLocks { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>>();
        static ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>> ChannelLocks { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>>();
        protected object GlobalCommandLock => CommandLocks.GetOrAdd(GetType(), new object());
        protected object GuildCommandLock => GuildLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong?, object>()).GetOrAdd(Context.Guild?.Id, new object());
        protected object ChannelCommandLock => ChannelLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong, object>()).GetOrAdd(Context.Channel.Id, new object());
        protected object InstanceCommandLock { get; } = new object();

        public static int TotalCommands { get; private set; } = 0;

        private bool HasReplied { get; set; }
        private IUserMessage AwaitMessage { get; set; }
        private ICommandContext Context { get; set; }

        protected virtual string DelayMessage => "COMMAND_DELAY_DEFAULT";
        protected virtual int DelayMessageMs => 3000;

        protected IUserMessage Message => Context?.Message;
        protected DiscordSocketClient Client => Context?.Client;
        protected IUser Author => Context?.Author;
        protected IMessageChannel Channel => Context?.Channel;
        protected SocketSelfUser BotUser => Client?.CurrentUser;
        protected IGuild Guild => Context?.Guild;
        protected GeneralGuildSetting GuildData => Context?.GuildData;
        protected GeneralUserSetting UserData => Context?.UserSetting;
        protected ValueFormatter Formatter => Context?.Formatter;
        protected IReplier Replier => Context?.Replier;
        protected ITextResourceCollection TextResource => Context?.TextResource;
        protected string Prefix => Context.Prefix;
        protected string CommandName => Context.CommandText;
        protected GeneralGlobalSetting GlobalSettings => SettingsManager.GetGlobalGroup<GeneralGlobalSetting>();
        protected IGuildUser GuildBotUser => Guild?.GetCurrentUserAsync().Result;
        protected IGuildUser GuildAuthor => Author as IGuildUser;
        protected IGuildChannel GuildChannel => Channel as IGuildChannel;
        protected string[] AcceptedPrefixes => new string[] { BotUser?.Mention, BotUser?.Username, GlobalSettings.DefaultPrefix, GuildData?.Prefix }
                                                    .Where(p => !string.IsNullOrWhiteSpace(p))
                                                    .ToArray();

        protected BotClient Bot { get; set; }
        protected ILogger Logger { get; private set; }
        protected ICommandService CommandService { get; private set; }
        protected ISettingsManager SettingsManager { get; private set; }
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

            Logger = factory.Get<ILogger>();
            Database = factory.Get<IDatabase>();
            Bot = factory.Get<BotClient>();
            CommandService = factory.Get<ICommandService>();
            Scheduler = factory.Get<IScheduler>();
            SettingsManager = factory.Get<ISettingsManager>();
            Downloader = factory.Get<IDownloader>();

            StartReplyTimer();

            Replier.OnSend += OnMessageSent;
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
                        AwaitMessage = Replier.Reply(Channel)
                                              .WithMessage(DelayMessage)
                                              .Send(true);
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
            => Replier.Reply(Channel);

        protected Task<IUserMessage> ReplyAsync(string message)
            => Reply.WithMessage(message).SendAsync();
        protected Task<IUserMessage> ReplyAsync(string message, ReplyType replyType)
            => Reply.WithMessage(message).SendAsync();
        protected Task<IUserMessage> ReplyAsync(string message, params object[] values)
            => Reply.WithMessage(message, values).SendAsync();
        protected Task<IUserMessage> ReplyAsync(string message, ReplyType replyType, params object[] values)
            => Reply.WithMessage(message, replyType, values).SendAsync();
        protected Task<IUserMessage> ReplyAsync(IEmbedable embed)
            => Reply.WithEmbedable(embed).SendAsync();
        protected Task<IUserMessage> ReplyAsync(EmbedBuilder embed)
            => ReplyAsync(Embedable.FromEmbed(embed));
    }
}
