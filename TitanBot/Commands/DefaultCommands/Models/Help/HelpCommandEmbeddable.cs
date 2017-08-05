using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Contexts;
using TitanBot.Formatting;
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

        private ITextResourceCollection TextResource => Context.TextResource;
        private string Prefix => Context.Prefix;
        private IUser BotUser => Context.Client.CurrentUser;

        private List<string> Usages { get; } = new List<string>();
        private string Usage { get; }
        private string Notes { get; }
        private string Aliases { get; }
        private string Group { get; }
        private string Flags { get; }
        private string NotesFooter { get; }

        public HelpCommandEmbeddable(CommandInfo subject, IEnumerable<CallInfo> permitted, string name, ICommandContext context)
        {
            Name = name;
            Command = subject;
            Permitted = permitted;
            Context = context;

            foreach (var call in Permitted)
                Usages.Add($"`{Context.Prefix}{Name} {call.SubCall} {string.Join(" ", call.GetParameters())} {call.GetFlags()}` - {TextResource.GetResource(call.Usage)}".RegexReplace(" +", " "));
            Usage = string.Join("\n", Usages);
            if (string.IsNullOrWhiteSpace(Usage))
                Usage = TextResource.GetResource(SINGLE_NOUSAGE);
            Notes = TextResource.GetResource(Command.Note);
            NotesFooter = TextResource.GetResource(SINGLE_USAGE_FOOTER);
            Aliases = Command.Alias.Length == 0 ? "" : string.Join(", ", Command.Alias.ToList());
            Group = Command.Group ?? TextResource.GetResource(SINGLE_NOGROUP);
            Flags = string.Join("\n", Permitted.SelectMany(c => c.Flags)
                                               .GroupBy(f => f.ShortKey)
                                               .Select(g => g.First().ToString(TextResource)));
        }

        public Embed GetEmbed()
        { 
            var builder = new LocalisedEmbedBuilder
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = (SINGLE_TITLE, ReplyType.Info,Command.Name),
                Timestamp = DateTime.Now,
                Footer = new LocalisedFooterBuilder().WithRawIconUrl(BotUser.GetAvatarUrl()).WithText(SINGLE_FOOTER, BotUser.Username),
            }.WithDescription(Command.Description ?? SINGLE_NODESCRIPTION);

            builder.AddInlineField(f => f.WithName(TBLocalisation.GROUP).WithRawValue(Group));
            if (!string.IsNullOrWhiteSpace(Aliases))
                builder.AddInlineField(f => f.WithName(TBLocalisation.ALIASES).WithRawValue(Format.Sanitize(Aliases)));
            builder.AddField(f => f.WithName(TBLocalisation.USAGE).WithRawValue(Usage));
            if (!string.IsNullOrWhiteSpace(Flags))
                builder.AddField(f => f.WithName(TBLocalisation.FLAGS).WithRawValue(Flags));
            if (!string.IsNullOrWhiteSpace(Notes))
                builder.AddField(f => f.WithName(TBLocalisation.NOTES).WithRawValue(Notes + (Usages.Count > 0 ? NotesFooter : "")));
            else if (Usages.Count > 0)
                builder.AddField(f => f.WithName(TBLocalisation.NOTES).WithRawValue(NotesFooter));

            return builder.Localise(TextResource);
        }

        public string GetString()
        {
            var msg = TextResource.Format(SINGLE_TITLE, ReplyType.Info, Command.Name) + "\n" +
                      TextResource.GetResource(Command.Description ?? SINGLE_NODESCRIPTION) + "\n" + 
                      $"**{TextResource.GetResource(TBLocalisation.GROUP)}**: {Group}\n";
            if (!string.IsNullOrWhiteSpace(Aliases))
                msg += $"**{TextResource.GetResource(TBLocalisation.ALIASES)}**: {Format.Sanitize(Aliases)}\n";
            msg += $"**{TextResource.GetResource(TBLocalisation.USAGE)}**:\n{Usage}\n";
            if (!string.IsNullOrWhiteSpace(Flags))
                msg += $"**{TextResource.GetResource(TBLocalisation.FLAGS)}**:\n{Flags}\n";
            if (!string.IsNullOrWhiteSpace(Notes))
                msg += $"**{TextResource.GetResource(TBLocalisation.NOTES)}**:\n{Notes}";
            if (Usages.Count != 0)
                msg += NotesFooter;

            return msg.Trim();
        }
    }
}
