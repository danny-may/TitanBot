using Discord;
using Discord.WebSocket;
using System;
using TitanBot.Database;
using TitanBot.Dependencies;
using TitanBot.Settings;
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

        internal CommandContext(IUserMessage message, IDependencyFactory factory)
        {
            DependencyFactory = factory;
            Client = DependencyFactory.Get<DiscordSocketClient>();
            Message = message;
            Channel = message.Channel;
            Author = message.Author;
            Guild = (message.Channel as IGuildChannel)?.Guild;
            if (Guild != null)
                GuildData = DependencyFactory.Get<ISettingsManager>().GetGroup<GeneralSettings>(Guild.Id);
        }

        public void CheckCommand(ICommandService commandService, string defaultPrefix)
        {
            if (Message.HasStringPrefix(defaultPrefix, out int prefixLength, StringComparison.InvariantCultureIgnoreCase) ||
                (GuildData?.Prefix != null && Message.HasStringPrefix(GuildData.Prefix, out prefixLength, StringComparison.InvariantCultureIgnoreCase)))
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
            CommandText = remaining.Substring(0, commandLength).Trim();
            ArgPos = prefixLength + commandLength;
        }

        public string[] SplitArguments(bool ignoreFlags, out (string Key, string Value)[] flags, int? maxLength = null, int? densePos = null)
        {
            var argText = Message.Content.Substring(ArgPos).Trim();
            return argText.SmartSplit(maxLength, densePos, ignoreFlags, out flags);
        }
    }
}
