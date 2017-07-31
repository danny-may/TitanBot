using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using TitanBot.Dependencies;
using TitanBot.Formatting;
using TitanBot.Settings;

namespace TitanBot.Commands
{
    class CommandContext : ICommandContext
    {
        public DiscordSocketClient Client { get; }
        public IUserMessage Message { get; }
        public IMessageChannel Channel { get; }
        public IUser Author { get; }
        public IGuild Guild { get; }

        public int ArgPos { get; private set; }
        public string Prefix { get; private set; }
        public string CommandText { get; private set; }
        public bool IsCommand => Command != null;
        public bool ExplicitPrefix { get; private set; }
        public CommandInfo? Command { get; set; }

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


        private Lazy<IReplier> _replier { get; }
        private Lazy<ITextResourceCollection> _textResource { get; }
        private Lazy<ITextResourceManager> _textManager { get; }
        private Lazy<ValueFormatter> _formatter { get; }
        private Lazy<ISettingContext> _globalSettings { get; }
        private Lazy<ISettingContext> _channelSettings { get; }
        private Lazy<ISettingContext> _userSettings { get; }
        private Lazy<ISettingContext> _guildSettings { get; }


        internal CommandContext(IUserMessage message, IDependencyFactory factory)
        {
            Client = factory.Get<DiscordSocketClient>();
            Message = message;
            Channel = message.Channel;
            Author = message.Author;
            Guild = (message.Channel as IGuildChannel)?.Guild;

            var settingManager = factory.GetOrStore<ISettingManager>();

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

        public void CheckCommand(ICommandService commandService, string defaultPrefix)
        {
            if ((GeneralGuildSetting?.Prefix != null && Message.HasStringPrefix(GeneralGuildSetting.Prefix, out int prefixLength, StringComparison.InvariantCultureIgnoreCase)) || 
                Message.HasStringPrefix(defaultPrefix, out prefixLength, StringComparison.InvariantCultureIgnoreCase))
                ExplicitPrefix = true;
            else if (Message.HasStringPrefix(Client.CurrentUser.Username + " ", out prefixLength, StringComparison.InvariantCultureIgnoreCase) ||
                     Message.HasMentionPrefix(Client.CurrentUser, out prefixLength))
                ExplicitPrefix = false;
            else if (Guild == null)
            {
                prefixLength = 0;
                ExplicitPrefix = false;
            }
            else
                return;

            Prefix = Message.Content.Substring(0, prefixLength).Trim();

            var remaining = Message.Content.Substring(prefixLength).TrimStart();
            Command = commandService.Search(remaining, out int commandLength);
            if (commandLength == 0)
                CommandText = remaining.Split(' ').First();
            else
                CommandText = remaining.Substring(0, commandLength).Trim();
            ArgPos = Message.Content.IndexOf(CommandText) + commandLength;
        }

        public string[] SplitArguments(bool ignoreFlags, out (string Key, string Value)[] flags, int? maxLength = null, int? densePos = null)
        {
            var argText = Message.Content.Substring(ArgPos).Trim();
            return argText.SmartSplit(maxLength, densePos, ignoreFlags, out flags);
        }
    }
}
