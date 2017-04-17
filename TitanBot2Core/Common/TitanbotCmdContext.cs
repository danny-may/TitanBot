using Discord.Commands;
using Discord.WebSocket;

namespace TitanBot2.Common
{
    public class TitanbotCmdContext : SocketCommandContext
    {
        public TitanBot TitanBot;

        public TitanbotCmdContext(TitanBot bot, SocketUserMessage msg) : base(bot._client, msg)
        {
            TitanBot = bot;
        }

        public bool CheckCommand(ref int argPos)
        {
            if (Message.Channel is SocketDMChannel)
            {
                Prefix = "";
                return true;
            }
            if (Message.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                Prefix = Client.CurrentUser.Mention;
                return true;
            }
            var config = Configuration.Instance;
            if (Message.HasStringPrefix(config.Prefix, ref argPos))
            {
                Prefix = config.Prefix;
                return true;
            }
            //var blockPrefix = TitanBotDb.Blocks.GetPrefix(msg.GetBlockId);
            //if (Message.HasStringPrefix(blockPrefix, ref argPos))
            //{
            //    prefix = blockPrefix;
            //    return true;
            //}

            return false;
        }

        public string Prefix { get; private set; }
    }
}
