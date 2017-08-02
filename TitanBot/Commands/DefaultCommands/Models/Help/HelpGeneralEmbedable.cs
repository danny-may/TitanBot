using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Contexts;
using TitanBot.Formatting;
using TitanBot.Replying;

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

        public Embed GetEmbed()
        {
            var builder = new LocalisedEmbedBuilder()
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = (TitanBotResource.HELP_LIST_TITLE, ReplyType.Info),
                Description = (TitanBotResource.HELP_LIST_DESCRIPTION, Prefix, string.Join("\", \"", AcceptedPrefixes)),
                Timestamp = DateTime.Now,
                Footer = new LocalisedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = (TitanBotResource.EMBED_FOOTER, BotUser.Username, "Help")
                }
            };

            var groups = Commands.GroupBy(c => c.Group);
            foreach (var group in groups)
                builder.AddField(group.Key, string.Join(", ", group.GroupBy(g => g.Name).Select(g => g.Key)));

            return builder.Localise(TextResource);
        }

        public string GetString()
        {
            var msg =  TextResource.GetResource(TitanBotResource.HELP_LIST_TITLE, ReplyType.Info) + "\n" +
                       TextResource.Format(TitanBotResource.HELP_LIST_DESCRIPTION, Prefix, string.Join("\", \"", AcceptedPrefixes)) + "\n" +
                       "```prolog\n";
            var groups = Commands.GroupBy(c => c.Group);
            foreach (var group in groups)
                msg += TextResource.Format(TitanBotResource.HELP_LIST_COMMAND, group.Key.ToTitleCase(), string.Join(", ", group.GroupBy(g => g.Name).Select(g => g.Key.ToLower()))) + "\n";

            return msg.Trim() + "```";
        }
    }
}
