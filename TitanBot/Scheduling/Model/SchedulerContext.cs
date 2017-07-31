using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TitanBot.Commands;
using TitanBot.Formatting;
using TitanBot.Settings;
using TitanBot.Dependencies;

namespace TitanBot.Scheduling
{
    class SchedulerContext : ISchedulerContext
    {
        public DiscordSocketClient Client { get; }
        private IDependencyFactory Factory { get; }
        public ISchedulerRecord Record { get; }

        public IUserMessage Message => _message.Value;
        public IMessageChannel Channel => _channel.Value;
        public IUser Author => _author.Value;
        public IGuild Guild => _guild.Value;

        public IReplier Replier => _replier.Value;
        public ITextResourceCollection TextResource => _textResource.Value;
        public ITextResourceManager TextManager => _textManager.Value;
        public ValueFormatter Formatter => _formatter.Value;

        public GeneralGlobalSetting GeneralGlobalSetting => GlobalSettings?.Get<GeneralGlobalSetting>();
        public GeneralGuildSetting GeneralGuildSetting => GuildSettings?.Get<GeneralGuildSetting>();
        public GeneralUserSetting GeneralUserSetting => UserSettings?.Get<GeneralUserSetting>();
        
        public ISettingContext GlobalSettings => _globalSettings.Value;
        public ISettingContext ChannelSettings => _channelSettings.Value;
        public ISettingContext UserSettings => _userSettings.Value;
        public ISettingContext GuildSettings => _guildSettings.Value;

        private Lazy<IUserMessage> _message { get; }
        private Lazy<IMessageChannel> _channel { get; }
        private Lazy<IGuild> _guild { get; }
        private Lazy<IUser> _author { get; }
        private Lazy<IReplier> _replier { get; }
        private Lazy<ITextResourceCollection> _textResource { get; }
        private Lazy<ITextResourceManager> _textManager { get; }
        private Lazy<ValueFormatter> _formatter { get; }
        private Lazy<ISettingContext> _globalSettings { get; }
        private Lazy<ISettingContext> _channelSettings { get; }
        private Lazy<ISettingContext> _userSettings { get; }
        private Lazy<ISettingContext> _guildSettings { get; }

        public SchedulerContext(SchedulerRecord record, DiscordSocketClient client, IDependencyFactory factory)
        {
            Client = client;
            Factory = factory;
            Record = record;

            var settingManager = factory.GetOrStore<ISettingManager>();

            _channel = new Lazy<IMessageChannel>(() => Record.ChannelId == null ? Author.GetOrCreateDMChannelAsync() as IMessageChannel 
                                                                                : Client.GetChannel(Record.ChannelId.Value) as IMessageChannel);
            _message = new Lazy<IUserMessage>(() => Record.MessageId == null ? null : Channel?.GetMessageAsync(Record.MessageId.Value).Result as IUserMessage);
            _author = new Lazy<IUser>(() => Client.GetUser(Record.UserId));
            _guild = new Lazy<IGuild>(() => Record.GuildId == null ? null : Client.GetGuild(Record.GuildId.Value));

            _globalSettings = new Lazy<ISettingContext>(() => settingManager.GetContext(settingManager.Global));
            _channelSettings = new Lazy<ISettingContext>(() => settingManager.GetContext(Channel));
            _guildSettings = new Lazy<ISettingContext>(() => Guild == null ? null : settingManager.GetContext(Guild));
            _userSettings = new Lazy<ISettingContext>(() => settingManager.GetContext(Author));
            _formatter = new Lazy<ValueFormatter>(() => factory.WithInstance(this)
                                                               .Construct<ValueFormatter>());
            _textManager = new Lazy<ITextResourceManager>(() => factory.GetOrStore<ITextResourceManager>());
            _textResource = new Lazy<ITextResourceCollection>(() => TextManager.GetForLanguage(GeneralGuildSetting?.PreferredLanguage ?? GeneralUserSetting.Language, Formatter));
            _replier = new Lazy<IReplier>(() => factory.WithInstance(this)
                                                       .Construct<IReplier>());
        }
    }
}
