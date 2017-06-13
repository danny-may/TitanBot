using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TitanBotBase.Database;
using TitanBotBase.Database.Tables;
using TitanBotBase.Util;

namespace TitanBotBase.Commands
{
    public class CommandContext : ICommandContext
    {
        public IMessageChannel Channel { get; }
        public IGuild Guild { get; }
        public IUserMessage Message { get; }
        public IUser Author { get; }
        public DiscordSocketClient Client { get; }
        public Guild GuildData { get; }
        public int ArgPos { get; private set; }
        public string Prefix { get; private set; }
        public string CommandText { get; private set; }
        public CommandInfo Command => _command.Value;
        public bool IsCommand => _command != null;
        public bool ExplicitPrefix { get; private set; }
        private CommandInfo? _command { get; set; }

        internal CommandContext(IUserMessage message, DiscordSocketClient client, IDatabase database)
        {
            Client = client;
            Message = message;
            Channel = message.Channel;
            Author = message.Author;
            Guild = (message.Channel as IGuildChannel)?.Guild;
            if (Guild != null)
                GuildData = database.AddOrGet(Guild.Id, () => new Guild()).Result;
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
            _command = commandService.Search(remaining, out int commandLength);
            CommandText = remaining.Substring(0, commandLength).Trim();
            ArgPos = prefixLength + commandLength;
        }

        public string[] SplitArguments(out (string Key, string Value)[] flags, int? maxLength = null, int? densePos = null)
        {
            var argText = Message.Content.Substring(ArgPos).Trim();
            return argText.SmartSplit(maxLength, densePos, out flags);
        }
    }
}
