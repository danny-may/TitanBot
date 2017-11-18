using Discord.WebSocket;
using Pihrtsoft.Text.RegularExpressions.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Titanbot.Commands.Interfaces;
using Titanbot.Commands.Models;
using Titanbot.Config;
using static Pihrtsoft.Text.RegularExpressions.Linq.Patterns;
using static System.Text.RegularExpressions.RegexOptions;

namespace Titanbot.Commands.Splitters
{
    public class MessageSplitter : IMessageSplitter
    {
        #region Fields

        protected BotConfig Config { get; }
        protected DiscordSocketClient Client { get; }

        protected virtual string Key_Prefix { get; } = "prefix";
        protected virtual string Key_CommandName { get; } = "command";
        protected virtual string Key_Arguments { get; } = "args";
        protected virtual string Key_Argument { get; } = "arg";
        protected virtual string Key_Flags { get; } = "flags";
        protected virtual string Key_Flag { get; } = "flag";
        protected virtual string Key_FlagKey { get; } = "flagK";
        protected virtual string Key_FlagValue { get; } = "flagV";

        protected Random Rand { get; }

        #endregion Fields

        #region Constructors

        public MessageSplitter(DiscordSocketClient client, BotConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Rand = new Random();
        }

        #endregion Constructors

        #region Methods

        protected virtual string[] GetPrefixes(SocketGuild guild)
        {
            var explicitPrefixes = new List<string>
            {
                Config.DefaultPrefix,
                "/" //Guild prefix
            };

            var naturalPrefixes = new List<string>
            {
                Client.CurrentUser.Username,
                Client.CurrentUser.Mention,
                "CoolBot" //Guild nickname
            };

            return explicitPrefixes.Concat(naturalPrefixes.Select(p => p + " "))
                                   .Where(p => !string.IsNullOrWhiteSpace(p))
                                   .Distinct()
                                   .ToArray();
        }

        #region Builder Methods

        protected virtual QuantifiablePattern WhiteSpaceOrStart()
            => AssertBack(WhiteSpace().Or(BeginInput()));

        protected virtual QuantifiablePattern WhiteSpaceOrEnd()
            => Assert(WhiteSpace().Or(WhiteSpace().MaybeMany() + EndInput()));

        protected virtual QuantifiablePattern QuoteBlock()
        {
            var id = Rand.Next((int)1e8, (int)1e9 - 1);
            var quoteId = $"quote_{id}";
            var escapeId = $"escape_{id}";
            return Group(NamedGroup(quoteId, Any(Character('"'), Character('\''), Character('`').Count(1, 3))) +
                         Group(Assert(NamedGroup(escapeId, Character('\\').Maybe())) +
                               GroupReference(escapeId) +
                               Any())
                         .MaybeMany()
                         .Lazy() +
                         GroupReference(quoteId));
        }

        protected virtual QuantifiablePattern Prefix(SocketGuild guild)
            => NamedGroup(Key_Prefix, Any(GetPrefixes(guild)));

        protected virtual QuantifiablePattern CommandName()
            => NamedGroup(Key_CommandName, NotWhiteSpaces());

        protected virtual QuantifiablePattern Argument()
            => NamedGroup(Key_Argument, WhiteSpaceOrStart() +
                                        NotAssert(Flag()) +
                                        Group(QuoteBlock()
                                                  .Or(Character('\\') + WhiteSpace())
                                                  .Or(Character(',') + WhiteSpace().MaybeMany())
                                                  .Or(Not(WhiteSpace())))
                                            .OneMany().Lazy() +
                                        WhiteSpaceOrEnd());

        protected virtual QuantifiablePattern Arguments()
            => NamedGroup(Key_Arguments, Group(WhiteSpaceOrStart() +
                                               Argument() +
                                               WhiteSpaceOrEnd())
                                            .OneMany());

        protected virtual QuantifiablePattern FlagKey()
            => NamedGroup(Key_FlagKey, WhiteSpaceOrStart() +
                                       Character('-').Count(1, 2) +
                                       WordChars() +
                                       WhiteSpaceOrEnd());

        protected virtual QuantifiablePattern FlagValue()
            => NamedGroup(Key_FlagValue, WhiteSpaceOrStart() +
                                         Group(QuoteBlock()
                                                    .Or(Character('\\') + Character('-'))
                                                    .Or(Character(',') + WhiteSpace().MaybeMany() + Character('-'))
                                                    .Or(Character('-') + WhiteSpaces())
                                                    .Or(Not(Character('-'))))
                                            .MaybeMany() +
                                         WhiteSpaceOrEnd());

        protected virtual QuantifiablePattern Flag()
            => NamedGroup(Key_Flag, WhiteSpaceOrStart() +
                                    FlagKey() +
                                    Group(WhiteSpaces() +
                                          FlagValue())
                                        .Maybe() +
                                    WhiteSpaceOrEnd());

        protected virtual QuantifiablePattern Flags()
            => NamedGroup(Key_Flags, Group(Flag() +
                                           WhiteSpaces().Or(EndInput()))
                                     .OneMany());

        protected virtual Pattern Command(SocketGuild guild)
            => BeginInput() +
               Prefix(guild) +
               WhiteSpace().MaybeMany() +
               CommandName() +
               Group(WhiteSpaces() + Arguments().Maybe() + WhiteSpace().MaybeMany() + Flags().Maybe()).Maybe() +
               EndInput();

        #endregion Builder Methods

        #endregion Methods

        #region IMessageSplitter

        public virtual bool TryParseMessage(SocketUserMessage message, out string prefix, out string commandName, out string rawArgs, out string[] args, out FlagValue[] flags)
        {
            prefix = commandName = rawArgs = null;
            args = null;
            flags = null;

            var regex = Command((message.Channel as SocketGuildChannel)?.Guild);

            var match = regex.Match(message.Content, Singleline | IgnoreCase);

            if (!match.Success)
                return false;

            prefix = match.Groups[Key_Prefix].Value;
            commandName = match.Groups[Key_CommandName].Value;

            rawArgs = match.Groups[Key_Arguments].Value.Trim();
            var rawFlags = match.Groups[Key_Flags].Value.Trim();

            var argRegex = Argument();
            var flagRegex = Flag();

            var argMatches = argRegex.Matches(rawArgs);
            var flagMatches = flagRegex.Matches(rawFlags);

            args = argMatches.Where(m => m.Success)
                             .Select(m => m.Groups[Key_Argument].Value)
                             .ToArray();

            flags = flagMatches.Where(m => m.Success) //ToDo: Split flags of form `-abc value` into `-a`, `-b`, `-c value`
                               .Select(m => new FlagValue(m.Groups[Key_FlagKey].Value, m.Groups[Key_FlagValue].Value))
                               .ToArray();

            return true;
        }

        #endregion IMessageSplitter
    }
}