using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using TitanBot.Formatting;

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
            var builder = new EmbedBuilder()
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = TextResource.GetResource("HELP_LIST_TITLE", ReplyType.Info),
                Description = TextResource.Format("HELP_LIST_DESCRIPTION", Prefix, string.Join("\", \"", AcceptedPrefixes)),
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = TextResource.Format("EMBED_FOOTER", BotUser.Username, "Help")
                }
            };

            var groups = Commands.GroupBy(c => c.Group);
            foreach (var group in groups)
                builder.AddField(group.Key, string.Join(", ", group.GroupBy(g => g.Name).Select(g => g.Key)));

            return builder.Build();
        }

        public string GetString()
        {
            var msg =  TextResource.GetResource("HELP_LIST_TITLE", ReplyType.Info) + "\n" +
                       TextResource.Format("HELP_LIST_DESCRIPTION", Prefix, string.Join("\", \"", AcceptedPrefixes)) + "\n" +
                       "```prolog\n";
            var groups = Commands.GroupBy(c => c.Group);
            foreach (var group in groups)
                msg += $"{group.Key.ToTitleCase()} Commands:\n   {string.Join(", ", group.GroupBy(g => g.Name).Select(g => g.Key.ToLower()))}\n";

            return msg.Trim() + "```";
        }
    }
}
