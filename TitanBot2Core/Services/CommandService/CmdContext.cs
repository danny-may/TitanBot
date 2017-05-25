using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.Database;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Services.CommandService
{
    public class CmdContext : SocketCommandContext
    {
        public Logger Logger { get; private set; }
        public BotClient BotClient { get; private set; }
        public BotDatabase Database { get; private set; }
        public string Prefix { get; private set; }
        public TimerService TimerService { get; private set; }
        public CachedWebClient WebClient { get; private set; }
        public TT2DataService TT2DataService { get; private set; }
        public BotCommandService CommandService { get; set; }
        public string[] Arguments { get; private set; }
        public string Command { get; private set; }
        public IMessageChannel SuggestionChannel
            => Dependencies.SuggestionChannel;
        public IMessageChannel BugChannel
            => Dependencies.BugChannel;

        public BotDependencies Dependencies { get; private set; }

        public CmdContext(BotDependencies args, SocketUserMessage msg) : base(args.Client, msg)
        {
            BotClient = args.BotClient;
            Database = args.Database;
            TimerService = args.TimerService;
            Logger = BotClient.Logger;
            WebClient = args.WebClient;
            TT2DataService = args.TT2DataService;

            Dependencies = args;
        }

        public async Task<int?> CheckCommand()
        {
            int argPos = 0;
            if (Message.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                Prefix = Client.CurrentUser.Mention;
            }
            else if (Message.Content.ToLower().StartsWith(Client.CurrentUser.Username.ToLower() + " "))
            {
                Prefix = Client.CurrentUser.Username;
            }
            else if (Message.HasStringPrefix(Configuration.Instance.Prefix, ref argPos))
            {
                Prefix = Configuration.Instance.Prefix;
            }
            else if (Channel is IGuildChannel)
            {
                try
                {
                    var blockPrefix = await Database.Guilds.GetPrefix((Channel as IGuildChannel).GuildId);
                    if (Message.HasStringPrefix(blockPrefix, ref argPos))
                    {
                        Prefix = blockPrefix;
                    }
                }
                catch (Exception ex)
                {
                    await Logger.Log(ex, "CheckCommand");
                }
            }
            else if (Message.Channel is IDMChannel)
            {
                Prefix = "";
            }

            if (Prefix == null)
                return null;

            var input = Message.GetArguments(argPos);
            Command = input.Length > 0 ? input[0] : null;
            Arguments = input.Length > 1 ? input.Skip(1).ToArray() : new string[0];
            
            return argPos;
        }
    }
}
