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
    class HelpCommandEmbeddable : IEmbedable
    {
        private string Name { get; }
        private CommandInfo Command { get; }
        private ICommandContext Context { get; }
        private IEnumerable<CallInfo> Permitted { get; }
        
        private string Prefix => Context.Prefix;
        private IUser BotUser => Context.Client.CurrentUser;

        private List<ILocalisable<string>> Usages { get; } = new List<ILocalisable<string>>();
        private List<ILocalisable<string>> Flags { get; } = new List<ILocalisable<string>>();
        private LocalisedString Usage { get; }
        private LocalisedString Notes { get; }
        private string Aliases { get; }
        private LocalisedString Group { get; }
        private LocalisedString Flag { get; }
        private LocalisedString NotesFooter { get; }

        public HelpCommandEmbeddable(CommandInfo subject, IEnumerable<CallInfo> permitted, string name, ICommandContext context)
        {
            Name = name;
            Command = subject;
            Permitted = permitted;
            Context = context;

            foreach (var call in Permitted)
                Usages.Add(new RawString("`{0}{1} {2} {3} {4}` - {5}", Context.Prefix, Name, call.SubCall, string.Join(" ", call.GetParameters()), call.GetFlags(), (LocalisedString)call.Usage));
            if (Usages.Count == 0)
                Usage = (LocalisedString)SINGLE_NOUSAGE;
            else
                Usage = new DynamicString(tr => string.Join("\n", Usages.Select(u => u.Localise(tr).RegexReplace(" +", " "))));
            Notes = (LocalisedString)Command.Note;
            NotesFooter = (LocalisedString)SINGLE_USAGE_FOOTER;
            Aliases = Command.Alias.Length == 0 ? "" : string.Join(", ", Command.Alias.ToList());
            Group = (RawString)Command.Group ?? (LocalisedString)SINGLE_NOGROUP;
            Flags.AddRange(Permitted.SelectMany(c => c.Flags)
                                    .GroupBy(f => f.ShortKey)
                                    .Select(g => g.First()));
            Flag = LocalisedString.Join("\n", Flags.ToArray());
        }

        public ILocalisable<EmbedBuilder> GetEmbed()
        { 
            var builder = new LocalisedEmbedBuilder
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = (SINGLE_TITLE, ReplyType.Info, Command.Name),
                Timestamp = DateTime.Now,
                Footer = new LocalisedFooterBuilder().WithRawIconUrl(BotUser.GetAvatarUrl()).WithText(SINGLE_FOOTER, BotUser.Username),
            }.WithDescription(Command.Description ?? SINGLE_NODESCRIPTION);

            builder.AddInlineField(f => f.WithName(TBLocalisation.GROUP).WithValue(Group));
            if (!string.IsNullOrWhiteSpace(Aliases))
                builder.AddInlineField(f => f.WithName(TBLocalisation.ALIASES).WithRawValue(Format.Sanitize(Aliases)));
            builder.AddField(f => f.WithName(TBLocalisation.USAGE).WithValue(Usage));
            if (Flags.Count != 0)
                builder.AddField(f => f.WithName(TBLocalisation.FLAGS).WithValue(Flags));
            if (Notes != null && !string.IsNullOrWhiteSpace(Notes.Key))
                builder.AddField(f => f.WithName(TBLocalisation.NOTES).WithValue(tr => Notes.Localise(tr) + (Usages.Count > 0 ? NotesFooter.Localise(tr) : "")));
            else if (Usages.Count > 0)
                builder.AddField(f => f.WithName(TBLocalisation.NOTES).WithValue(NotesFooter));

            return builder;
        }

        public ILocalisable<string> GetString()
        {
            var baseFormat = "**{0}**: {1}";

            var entries = new List<LocalisedString>
            {
                (SINGLE_TITLE, ReplyType.Info, Command.Name),
                (LocalisedString)(Command.Description ?? SINGLE_NODESCRIPTION),
                new RawString(baseFormat, (LocalisedString)TBLocalisation.GROUP, Group)
            };
            if (!string.IsNullOrWhiteSpace(Aliases))
                entries.Add(new RawString(baseFormat, (LocalisedString)TBLocalisation.ALIASES, Format.Sanitize(Aliases)));
            entries.Add(new RawString(baseFormat, (LocalisedString)TBLocalisation.USAGE, Usage));
            if (Flags.Count != 0)
                entries.Add(new RawString(baseFormat, (LocalisedString)TBLocalisation.FLAGS, Flag));
            if (Notes != null && !string.IsNullOrWhiteSpace(Notes.Key))
                entries.Add(new RawString(baseFormat, (LocalisedString)TBLocalisation.NOTES, Notes));
            if (Usages.Count > 0)
                entries.Add(NotesFooter);

            return LocalisedString.Join("\n", entries.ToArray());
        }
    }
}
