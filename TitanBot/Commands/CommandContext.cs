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
        public IMessageChannel Channel { get; }
        public IDependencyFactory DependencyFactory { get; }
        public IGuild Guild { get; }
        public IUserMessage Message { get; }
        public IUser Author { get; }
        public DiscordSocketClient Client { get; }
        public GeneralSettings GuildData { get; }
        public int ArgPos { get; private set; }
        public string Prefix { get; private set; }
        public string CommandText { get; private set; }
        public bool IsCommand => Command != null;
        public bool ExplicitPrefix { get; private set; }
        public CommandInfo? Command { get; set; }
        public IReplier Replier { get; }
        public ITextResourceCollection TextResource { get; }
        public ValueFormatter Formatter { get; }

        internal CommandContext(IUserMessage message, IDependencyFactory factory)
        {
            DependencyFactory = factory;
            Client = DependencyFactory.Get<DiscordSocketClient>();
            Message = message;
            Channel = message.Channel;
            Author = message.Author;
            Guild = (message.Channel as IGuildChannel)?.Guild;
            var database = DependencyFactory.Get<IDatabase>();
            var userdata = database.AddOrGet(Author.Id, () => new UserSetting()).Result;
            if (Guild != null)
                GuildData = DependencyFactory.Get<ISettingsManager>().GetGroup<GeneralSettings>(Guild.Id);
            TextResource = DependencyFactory.Get<ITextResourceManager>()
                                            .GetForLanguage(GuildData?.PreferredLanguage ?? userdata.Language);
            Formatter = DependencyFactory.WithInstance(userdata.AltFormat)
                                         .WithInstance(this)
                                         .Construct<ValueFormatter>();
            Replier = DependencyFactory.WithInstance(TextResource)
                                       .WithInstance(Formatter)
                                       .Construct<IReplier>();
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
