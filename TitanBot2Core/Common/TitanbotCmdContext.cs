using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Database;
using TitanBot2.Extensions;

namespace TitanBot2.Common
{
    public class TitanbotCmdContext : SocketCommandContext
    {
        public TitanBot TitanBot { get; }
        public Logger Logger { get; }

        public TitanbotCmdContext(TitanBot bot, SocketUserMessage msg) : base(bot.Client, msg)
        {
            TitanBot = bot;
            Logger = TitanBot.Logger;
        }

        public async Task<int?> CheckCommand()
        {
            int argPos = 0;
            if (Message.Channel is SocketDMChannel)
            {
                Prefix = "";
                return 0;
            }
            if (Message.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                Prefix = Client.CurrentUser.Mention;
                return argPos;
            }
            if (Message.HasStringPrefix(Client.CurrentUser.Username, ref argPos))
            {
                Prefix = Client.CurrentUser.Username;
                return argPos;
            }
            var config = Configuration.Instance;
            if (Message.HasStringPrefix(config.Prefix, ref argPos))
            {
                Prefix = config.Prefix;
                return argPos;
            }
            if (Channel is IGuildChannel)
            {
                var blockPrefix = await TitanbotDatabase.Guilds.GetPrefix((Channel as IGuildChannel).GuildId);
                if (Message.HasStringPrefix(blockPrefix, ref argPos))
                {
                    Prefix = blockPrefix;
                    return argPos;
                }
            }

            return null;
        }

        public string Prefix { get; private set; }
    }
}
