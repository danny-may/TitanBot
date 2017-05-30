using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService.Flags;
using TitanBot2.Services.Database;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Services.CommandService
{
    public class CmdContext : SocketCommandContext
    {
        public Logger Logger { get; private set; }
        public BotClient BotClient { get; private set; }
        public BotDatabase Database { get; private set; }
        public string Prefix => _splitter.Prefix;
        public bool ExplicitCommand => _splitter.HardPrefix;
        public TimerService TimerService { get; private set; }
        public CachedWebClient WebClient { get; private set; }
        public TT2DataService TT2DataService { get; private set; }
        public BotCommandService CommandService { get; set; }
        public string[] Arguments => _splitter.Arguments;
        public string Command => _splitter.Command;
        public FlagValue[] Flags => _splitter.Flags;
        public IMessageChannel SuggestionChannel
            => Dependencies.SuggestionChannel;
        public IMessageChannel BugChannel
            => Dependencies.BugChannel;

        private CmdSplitter _splitter;

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

            _splitter = new CmdSplitter(Database, Message, Client.CurrentUser, User, Channel);
        }

        public bool IsCommand()
        {
            return Prefix != null && Command != null;
        }
    }
}
