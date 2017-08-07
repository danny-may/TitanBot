using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Contexts;
using TitanBot.Formatting;
using TitanBot.Formatting.Interfaces;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Commands.HelpText;

namespace TitanBot.Commands
{
    class HelpGeneralEmbedable : IEmbedable
    {
        private IEnumerable<CommandInfo> Commands { get; }
        private ICommandContext Context { get; }
        private string[] AcceptedPrefixes { get; }

        private ITextResourceCollection TextResource => Context.TextResource;
        private string Prefix => Context.Prefix;
        private IUser BotUser => Context.Client.CurrentUser;

        public HelpGeneralEmbedable(IEnumerable<CommandInfo> commands, ICommandContext context, string[] acceptedPrefixes)
        {
            Commands = commands;
            Context = context;
            AcceptedPrefixes = acceptedPrefixes;
        }

        public ILocalisable<EmbedBuilder> GetEmbed()
        {
            var builder = new LocalisedEmbedBuilder()
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = (LIST_TITLE, ReplyType.Info),
                Description = (LIST_DESCRIPTION, Prefix, string.Join("\", \"", AcceptedPrefixes)),
                Timestamp = DateTime.Now,
                Footer = new LocalisedFooterBuilder().WithRawIconUrl(BotUser.GetAvatarUrl()).WithText((SINGLE_FOOTER, BotUser.Username))
            };

            var groups = Commands.GroupBy(c => c.Group);
            foreach (var group in groups)
                builder.AddField(f => f.WithRawName(group.Key).WithRawValue(string.Join(", ", group.GroupBy(g => g.Name).Select(g => g.Key))));

            return builder;
        }

        public ILocalisable<string> GetString()
        {
            var entries = new List<LocalisedString>
            {
                (LIST_TITLE, ReplyType.Info),
                (LIST_DESCRIPTION, Prefix, string.Join("\", \"", AcceptedPrefixes)),
                (RawString)"```prolog"
            };
            var groups = Commands.GroupBy(c => c.Group);
            foreach (var group in groups)
                entries.Add((LIST_COMMAND, group.Key.ToTitleCase(), string.Join(", ", group.GroupBy(g => g.Name).Select(g => g.Key.ToLower()))));
            entries.Add((RawString)"```");

            return LocalisedString.Join("\n", entries.ToArray());
        }
    }
}
