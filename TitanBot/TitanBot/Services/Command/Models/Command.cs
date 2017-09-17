using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services.Command;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Formatting.Models;
using TitanBot.Core.Services.Messaging;

namespace TitanBot.Services.Command.Models
{
    public abstract class Command : ICommand
    {
        //locks
        private static ConcurrentDictionary<Type, object> CommandLocks { get; } = new ConcurrentDictionary<Type, object>();
        private static ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>> GuildLocks { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong?, object>>();
        private static ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>> ChannelLocks { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<ulong, object>>();
        protected object GlobalCommandLock => CommandLocks.GetOrAdd(GetType(), new object());
        protected object GuildCommandLock => GuildLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong?, object>()).GetOrAdd(Context.Guild?.Id, new object());
        protected object ChannelCommandLock => ChannelLocks.GetOrAdd(GetType(), new ConcurrentDictionary<ulong, object>()).GetOrAdd(Context.Channel.Id, new object());
        protected object InstanceCommandLock { get; } = new object();

        public static int TotalCommands { get; private set; } = 0;

        private bool HasReplied { get; set; }
        private IUserMessage AwaitMessage { get; set; }

        protected virtual IDisplayable<string> DelayMessage => TransKey.From("DELAY_DEFAULT");
        protected virtual int DelayMessageMs => 3000;

        protected SocketUserMessage Message => Context?.Message as SocketUserMessage;
        protected DiscordSocketClient Discord => Context?.Discord as DiscordSocketClient;
        protected SocketUser Author => Context?.User as SocketUser;
        protected SocketChannel Channel => Context?.Channel as SocketChannel;
        protected SocketSelfUser BotUser => Discord?.CurrentUser;
        protected SocketGuild Guild => Context?.Guild as SocketGuild;

        protected IValueFormatter ValueFormatter => Context?.ValueFormatter;
        private IMessageService MessageService => Context?.MessageService;
        protected ITranslationSet TranslationSet => Context?.TranslationSet;
        protected string Prefix => Context.Prefix;
        protected string CommandName => Context.CommandText;
        //protected ISettingContext GlobalSettings => SettingsManager.GetContext(SettingsManager.Global);
        //protected ISettingContext GuildSettings => Guild == null ? null : SettingsManager.GetContext(Guild);
        //protected ISettingContext UserSettings => SettingsManager.GetContext(Author);
        //protected ISettingContext ChannelSettings => SettingsManager.GetContext(Channel);
        //protected GeneralGlobalSetting GeneralGlobalSetting => GlobalSettings.Get<GeneralGlobalSetting>();
        //protected GeneralGuildSetting GeneralGuildSetting => GuildSettings?.Get<GeneralGuildSetting>();
        //protected GeneralUserSetting GeneralUserSetting => UserSettings.Get<GeneralUserSetting>();
        protected IGuildUser GuildBotUser => Guild?.CurrentUser;
        protected IGuildUser GuildAuthor => Author as IGuildUser;
        protected IGuildChannel GuildChannel => Channel as IGuildChannel;
        protected string[] AcceptedPrefixes => new string[] { BotUser?.Mention, BotUser?.Username }//, GeneralGlobalSetting.DefaultPrefix, GeneralGuildSetting?.Prefix }
                                                    .Where(p => !string.IsNullOrWhiteSpace(p))
                                                    .ToArray();

        protected ICommandContext Context { get; set; }

        public Command()
        {
            TotalCommands++;
        }

        public virtual void Install(ICommandContext context)
        {
            Context = context;

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
                        AwaitMessage = MessageService.Send(Channel as IMessageChannel, Author)
                                              .WithText(DelayMessage)
                                              .SendAsync().Result;
                    }
                }
            });
        }
    }
}