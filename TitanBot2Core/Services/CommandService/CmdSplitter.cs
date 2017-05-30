using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Flags;
using TitanBot2.Services.Database;

namespace TitanBot2.Services.CommandService
{
    public class CmdSplitter
    {
        public string[] Arguments { get; private set; } = new string[0];
        public FlagValue[] Flags { get; private set; } = new FlagValue[0];
        public string Prefix { get; private set; } = null;
        public bool HardPrefix { get; private set; } = false;
        public string Command { get; private set; } = null;

        private BotDatabase _database;
        private IUserMessage _message;
        private IUser _user;
        private IChannel _channel;
        private IUser _me;

        public CmdSplitter(BotDatabase database, IUserMessage message, IUser me, IUser user, IChannel channel)
        {
            _database = database;
            _message = message;
            _user = user;
            _channel = channel;
            _me = me;

            Parse().Wait();
        }

        private async Task Parse()
        {
            var prefix = await GetPrefix();
            
            if (prefix == null)
                return;

            var hardprefix = !(prefix.Item2 == _me.Mention || prefix.Item2 == _me.Username || prefix.Item2 == "");
            Process(_message.Content, out List<string> args, out List<FlagValue> flags);

            if (args.Count == 0 || (args.First() == prefix.Item2 && args.Count == 1))
                return;

            Prefix = prefix.Item2;
            HardPrefix = hardprefix;
            Flags = flags.ToArray();

            if (args.First().Length == prefix.Item1)
            {
                Command = args[1];
                args.RemoveAt(1);
            }
            else
                Command = args[0].Substring(prefix.Item1);

            args.RemoveAt(0);

            Arguments = args.ToArray();
        }

        private async Task<Tuple<int, string>> GetPrefix()
        {
            int argpos = 0;
            if (_message.HasMentionPrefix(_me, ref argpos))
                return Tuple.Create(argpos-1, _me.Mention);
            if (_message.HasStringPrefix(_me.Username + " ", ref argpos, StringComparison.InvariantCultureIgnoreCase))
                return Tuple.Create(argpos-1, _me.Username);
            if (_message.HasStringPrefix(Configuration.Instance.Prefix, ref argpos, StringComparison.InvariantCultureIgnoreCase))
                return Tuple.Create(argpos, Configuration.Instance.Prefix);
            if (_channel is IGuildChannel)
            {
                var blockPrefix = await _database.Guilds.GetPrefix((_channel as IGuildChannel).GuildId);
                if (_message.HasStringPrefix(blockPrefix))
                {
                    return Tuple.Create(blockPrefix.Length, blockPrefix);
                }
            }
            if (_message.Channel is IDMChannel)
                return Tuple.Create(0, "");

            return null;
        }

        private void Process(string text, out List<string> args, out List<FlagValue> flags)
        {
            var blocks = GetBlocks(text);

            args = blocks.TakeWhile(b => !b.StartsWith("-")).ToList();
            flags = new List<FlagValue>();

            var flagBlocks = blocks.SkipWhile(b => !b.StartsWith("-"));

            foreach (var block in flagBlocks)
            {
                if (block.Length < 2 || block[0] != '-')
                    continue;

                var split = block.Split(new char[] { ' ' }, 2);

                if (split.Length == 1)
                    split = new string[] { split[0], null };

                if (split[0].StartsWith("--"))
                    flags.Add(new FlagValue(split[0].Substring(2), split[1]));
                else
                {
                    foreach (var flag in split[0].Skip(1).Take(split[0].Length - 2))
                    {
                        flags.Add(new FlagValue(flag.ToString(), null));
                    }
                    flags.Add(new FlagValue(split[0].Last().ToString(), split[1]));
                }
            }
        }

        private string[] GetBlocks(string text)
        {
            var isEscaped = false;
            var isQuoted = false;
            var isArray = false;
            var isFlag = false;
            var prevWasSpace = false;

            var blocks = new List<string>();
            var currentBlock = new StringBuilder();

            foreach (var c in text)
            {
                if (isEscaped)
                {
                    isEscaped = false;
                    currentBlock.Append(c);
                }
                else
                    switch (c)
                    {
                        case '\\':
                            isEscaped = true;
                            break;
                        case '"':
                            isQuoted = !isQuoted;
                            break;
                        case ',':
                            isArray = true;
                            currentBlock.Append(c);
                            break;
                        case '-':
                            if (isFlag && (prevWasSpace && !isQuoted && !isArray))
                            {
                                blocks.Add(currentBlock.ToString());
                                currentBlock = new StringBuilder();
                            }
                            currentBlock.Append(c);
                            isFlag = isFlag || (prevWasSpace && !isQuoted && !isArray);
                            break;
                        case ' ':
                            if (isQuoted || isArray || isFlag)
                                currentBlock.Append(c);
                            else
                            {
                                blocks.Add(currentBlock.ToString());
                                currentBlock = new StringBuilder();
                            }
                            if (isArray)
                                isArray = false;
                            break;
                        default:
                            currentBlock.Append(c);
                            break;
                    }
                prevWasSpace = c == ' ';
            }

            blocks.Add(currentBlock.ToString());

            return blocks.ToArray();
        }
    }
}
