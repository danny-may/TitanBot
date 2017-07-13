using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Storage;
using TitanBot.Dependencies;
using TitanBot.Downloader;
using TitanBot.Formatter;
using TitanBot.Logging;
using TitanBot.Scheduling;
using TitanBot.Settings;
using TitanBot.TextResource;

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

        protected virtual string DelayMessage => "This seems to be taking longer than expected...";
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
        protected OutputFormatter Formatter { get; private set; }
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
            Logger = factory.Get<ILogger>();
            Database = factory.Get<IDatabase>();
            Bot = factory.Get<BotClient>();
            CommandService = factory.Get<ICommandService>();
            Scheduler = factory.Get<IScheduler>();
            Replier = factory.GetOrStore<IReplier>();
            SettingsManager = factory.Get<ISettingsManager>();
            Downloader = factory.Get<IDownloader>();
            if (Guild != null)
                GuildData = SettingsManager.GetGroup<GeneralSettings>(Guild.Id);
            var userData = Database.AddOrGet(context.Author.Id, () => new UserSetting()).Result;
            Formatter = factory.WithInstance(userData.AltFormat)
                               .WithInstance(context)
                               .Construct<OutputFormatter>();
            TextResource = factory.GetOrStore<ITextResourceManager>()
                                  .GetForLanguage(Guild == null ? userData.Language : GuildData.PreferredLanguage);
            Prefix = context.Prefix;
            CommandName = context.CommandText;
            StartReplyCountdown();
        }

        void StartReplyCountdown()
        {
            Task.Run(async () =>
            {
                await Task.Delay(DelayMessageMs);
                lock (InstanceCommandLock)
                {
                    if (!HasReplied)
                        AwaitMessage = Replier.Reply(Channel, Author, DelayMessage);
                }
            });
        }

        void RegisterReply()
        {
            lock (InstanceCommandLock)
            {
                HasReplied = true;
                if (AwaitMessage != null)
                {
                    var temp = AwaitMessage;
                    AwaitMessage = null;
                    temp.DeleteAsync().Wait();
                }
            }
        }

        protected Task<IUserMessage> ReplyAsync(IMessageChannel channel, string message, ReplyType replyType, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            RegisterReply();
            return Replier.ReplyAsync(channel, Author, message, replyType, isTTS: isTTS, embed: embed, options: options);
        }
        protected IUserMessage Reply(IMessageChannel channel, string message, ReplyType replyType, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            RegisterReply();
            return Replier.Reply(channel, Author, message, replyType, isTTS: isTTS, embed: embed, options: options);
        }

        protected Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(Channel, message, ReplyType.None, isTTS, embed, options);
        
        protected Task<IUserMessage> ReplyAsync(string message, ReplyType replyType, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(Channel, message, replyType, isTTS, embed, options);
        
        protected Task<IUserMessage> ReplyAsync(IMessageChannel channel, string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(channel, message, ReplyType.None, isTTS, embed, options);
        
        protected Task<IUserMessage> ReplyAsync(IUser user, string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(user.GetOrCreateDMChannelAsync().Result, message, ReplyType.None, isTTS, embed, options);
        
        protected Task<IUserMessage> ReplyAsync(IUser user, string message, ReplyType replyType, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(user.GetOrCreateDMChannelAsync().Result, message, replyType, isTTS, embed, options);

        protected IUserMessage Reply(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => Reply(Channel, message, ReplyType.None, isTTS, embed, options);
        
        protected IUserMessage Reply(string message, ReplyType replyType, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => Reply(Channel, message, replyType, isTTS, embed, options);
        
        protected IUserMessage Reply(IMessageChannel channel, string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => Reply(channel, message, ReplyType.None, isTTS, embed, options);        
        
        protected IUserMessage Reply(IUser user, string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => Reply(user.GetOrCreateDMChannelAsync().Result, message, ReplyType.None, isTTS, embed, options);
        
        protected IUserMessage Reply(IUser user, string message, ReplyType replyType, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => Reply(user.GetOrCreateDMChannelAsync().Result, message, replyType, isTTS, embed, options);
    }
}
