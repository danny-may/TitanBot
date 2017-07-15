using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using TitanBot.Dependencies;
using TitanBot.Formatting;
using TitanBot.Settings;
using TitanBot.Storage;
using TitanBot.Util;

namespace TitanBot.Commands
{
    public class CommandContext : ICommandContext
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
        public ValueFormatter Formatter => _formatter.Value;
        public GeneralSettings GuildData => _guildData.Value;
        public UserSetting UserSetting => _userSetting.Value;

        private Lazy<IReplier> _replier { get; }
        private Lazy<ITextResourceCollection> _textResource { get; }
        private Lazy<ValueFormatter> _formatter { get; }
        private Lazy<GeneralSettings> _guildData { get; }
        private Lazy<UserSetting> _userSetting { get; }


        internal CommandContext(IUserMessage message, IDependencyFactory factory)
        {
            Client = factory.Get<DiscordSocketClient>();
            Message = message;
            Channel = message.Channel;
            Author = message.Author;
            Guild = (message.Channel as IGuildChannel)?.Guild;

            _userSetting = new Lazy<UserSetting>(() => factory.Get<IDatabase>()
                                                              .AddOrGet(Author.Id, () => new UserSetting { Id = Author.Id })
                                                              .Result);
            _guildData = new Lazy<GeneralSettings>(() => Guild != null ? factory.Get<ISettingsManager>().GetGroup<GeneralSettings>(Guild.Id) : null);
            _formatter = new Lazy<ValueFormatter>(() => factory.WithInstance(this)
                                                               .Construct<ValueFormatter>());
            _textResource = new Lazy<ITextResourceCollection>(() => factory.Get<ITextResourceManager>()
                                                                           .GetForLanguage(GuildData?.PreferredLanguage ?? UserSetting.Language, Formatter));
            _replier = new Lazy<IReplier>(() => factory.WithInstance(this)
                                                       .Construct<IReplier>());
        }

        public void CheckCommand(ICommandService commandService, string defaultPrefix)
        {
            if ((GuildData?.Prefix != null && Message.HasStringPrefix(GuildData.Prefix, out int prefixLength, StringComparison.InvariantCultureIgnoreCase)) || 
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
