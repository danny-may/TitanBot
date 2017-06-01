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
using TitanBot2.Services.CommandService.Models;
using TitanBot2.Services.Database;
using TitanBot2.Services.Database.Tables;
using TitanBot2.Services.Scheduler;
using TitanBot2.TypeReaders;

namespace TitanBot2.Services.CommandService
{
    public class CmdContext : SocketCommandContext
    {
        public Logger Logger { get; private set; }
        public BotClient BotClient { get; private set; }
        public BotDatabase Database { get; private set; }
        public string Prefix { get; private set; }
        public bool ExplicitCommand { get; private set; }
        public TimerService TimerService { get; private set; }
        public CachedWebClient WebClient { get; private set; }
        public TT2DataService TT2DataService { get; private set; }
        public BotCommandService CommandService { get; private set; }
        public TypeReaderCollection.CachedReader Readers { get; private set; }
        public string[] Arguments { get; private set; }
        public CommandInfo Command { get; private set; }
        public FlagValue[] Flags { get; private set; }
        public BotDependencies Dependencies { get; private set; }
        public Guild GuildData
        {
            get
            {
                if (_guildData == null)
                    _guildData = Database.Guilds.GetGuild(Guild?.Id ?? 0).Result;
                return _guildData;
            }
        }


        private Guild _guildData;
        private CmdSplitter _splitter;

        public CmdContext(BotDependencies args, BotCommandService cmdService, SocketUserMessage msg) : base(args.Client, msg)
        {
            BotClient = args.BotClient;
            Database = args.Database;
            TimerService = args.TimerService;
            Logger = BotClient.Logger;
            WebClient = args.WebClient;
            CommandService = cmdService;
            Readers = cmdService.GetReader();
            TT2DataService = args.TT2DataService;

            Dependencies = args;

            _splitter = new CmdSplitter(Database, CommandService, Client.CurrentUser);

            _splitter.TrySplit(msg, out string prefix, out bool hardPrefix, out CommandInfo cmd, out string[] arguments, out FlagValue[] flags);
            Prefix = prefix;
            ExplicitCommand = hardPrefix;
            Command = cmd;
            Arguments = arguments;
            Flags = flags;
        }

        public bool IsCommand()
        {
            return Prefix != null;
        }
    }
}
