using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using TitanBot2.Handlers;
using TitanBot2.Services;
using TitanBot2.Services.Database;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Common
{
    public class TitanbotCmdContext : SocketCommandContext
    {
        public Logger Logger { get; }
        public TitanBot TitanBot { get; }
        public TitanbotDatabase Database { get; }
        public string Prefix { get; private set; }
        public TimerService TimerService { get; }
        public CachedWebClient WebClient { get; }
        public TT2DataService TT2DataService { get; }

        public TitanbotDependencies Dependencies { get; }

        public TitanbotCmdContext(TitanbotDependencies args, SocketUserMessage msg) : base(args.Client, msg)
        {
            TitanBot = args.TitanBot;
            Database = args.Database;
            TimerService = args.TimerService;
            Logger = TitanBot.Logger;
            WebClient = args.WebClient;
            TT2DataService = args.TT2DataService;

            Dependencies = args;
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
                try
                {
                    var blockPrefix = await Database.Guilds.GetPrefix((Channel as IGuildChannel).GuildId);
                    if (Message.HasStringPrefix(blockPrefix, ref argPos))
                    {
                        Prefix = blockPrefix;
                        return argPos;
                    }
                }
                catch (Exception ex)
                {
                    await Logger.Log(ex, "CheckCommand");
                }
            }

            return null;
        }
    }
}
