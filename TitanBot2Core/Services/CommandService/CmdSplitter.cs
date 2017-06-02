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
using TitanBot2.Services.CommandService.Models;
using TitanBot2.Services.Database;

namespace TitanBot2.Services.CommandService
{
    internal class CmdSplitter
    {
        private BotDatabase _database;
        private IUser _me;
        private BotCommandService _cmdService;

        public CmdSplitter(BotDatabase database, BotCommandService cmdService, IUser me)
        {
            _database = database;
            _me = me;
            _cmdService = cmdService;
        }

        public void TrySplit(IUserMessage message, out string prefix, out bool hardPrefix, out CommandInfo command, out string[] arguments, out FlagValue[] flags)
        {
            prefix = null;
            hardPrefix = false;
            command = null;
            arguments = null;
            flags = null;
            var cmdStart = FindCommandStart(message).Result;
            if (cmdStart.Item1 == null)
                return;
            var text = message.Content.Substring(cmdStart.Item1.Value);
            var blocks = GetBlocks(text);

            prefix = message.Content.Substring(0, cmdStart.Item1.Value);
            hardPrefix = cmdStart.Item2;
            var cmd = blocks.FirstOrDefault();
            var cmdDef = _cmdService.Commands.FirstOrDefault(c => c.Name.ToLower() == cmd.ToLower() || c.Alias.Select(a => a.ToLower()).Contains(cmd.ToLower()));
            command = cmdDef;

            if (cmdDef == null)
                return;

            if (cmdDef.Calls.Any(c => c.Flags.Length > 0))
            {
                arguments = blocks.Skip(1).TakeWhile(a => !a.StartsWith("-")).ToArray();
                flags = ProcessFlags(blocks.Skip(1).SkipWhile(a => !a.StartsWith("-")).ToArray());
            }
            else
            {
                arguments = blocks.Skip(1).ToArray();
                flags = new FlagValue[0];
            }

            return;
        }

        internal async Task<ValueTuple<int?, bool>> FindCommandStart(IUserMessage message)
        {
            int argpos = 0;
            if (message.HasMentionPrefix(_me, ref argpos))
                return ValueTuple.Create((int?)argpos, false);
            if (message.HasStringPrefix(_me.Username + " ", ref argpos, StringComparison.InvariantCultureIgnoreCase))
                return ValueTuple.Create((int?)argpos, false);
            if (message.HasStringPrefix(Configuration.Instance.Prefix, ref argpos, StringComparison.InvariantCultureIgnoreCase))
                return ValueTuple.Create((int?)argpos, true);
            if (message.Channel is IGuildChannel)
            {
                var blockPrefix = await _database.Guilds.GetPrefix((message.Channel as IGuildChannel).GuildId);
                if (message.HasStringPrefix(blockPrefix, ref argpos))
                {
                    return ValueTuple.Create((int?)argpos, true);
                }
            }
            if (message.Channel is IDMChannel)
                return ValueTuple.Create((int?)0, false);

            return ValueTuple.Create((int?)null, false);
        }

        internal FlagValue[] ProcessFlags(IList<string> blocks)
        {
            var flags = new List<FlagValue>();

            foreach (var block in blocks)
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
                    flags.Add(new FlagValue(split[0].Last().ToString(), split[1].Trim()));
                }
            }

            return flags.ToArray();
        }

        internal string[] GetBlocks(string text)
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
                            else if (prevWasSpace)
                                continue;
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
