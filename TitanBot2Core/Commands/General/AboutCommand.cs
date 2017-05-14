using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.General
{
    public class AboutCommand : Command
    {
        public AboutCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ShowAbout());
            Usage.Add("`{0}` - Shows some about text for me");
            Description = "A tiny command that just displays some helpful links :";
            _specialThanks = new Dictionary<ulong, string>
            {
                {181829392027090954, "Awesome guy, helped out with the initial testing of titanbot" },
                {196387723018240000, $"Kept pestering {Context?.Client.GetUser(135556895086870528).Username} for things to be added" },
                {257960645725650954, "Did all the images for the equipment" },
                {130084593515626496, "Made my avatar and makes other cool things" }
            };
        }

        private Dictionary<ulong, string> _specialThanks;

        public async Task ShowAbout()
        {
            var message = $"'Ello there :wave:, Im Titanbot! Im an open source discord bot dedicated to TT2 written in C# by {Context.Client.GetUser(135556895086870528).Username}.\n" +
                          $"I can do a variety of things, all of which are better documented in my `{Context.Prefix}help` command!\n\n" +
                          $"Im always going to be taking suggestions, which can either be DM'd to {Context.Client.GetUser(135556895086870528).Username} (He has a terrible memory though) Or sent in the #suggestions channel on my support server!\n\n" +
                           "Public repository: <https://github.com/Titansmasher/TitanBot2>\n" +
                           "Support server: <https://discord.gg/abQPtHu>\n\n" +
                           ":sparkles: **Special thanks to** :sparkles:\n";
            var count = 1;
            foreach (var thanks in _specialThanks)
            {
                message = message + $"  {count++}. {Context.Client.GetUser(thanks.Key).Username}#{Context.Client.GetUser(thanks.Key).Discriminator} - {thanks.Value}\n";
            }
            message = message + $"  {count++}. Everyone from the {Context.Client.GetGuild(255058878268571649)?.Name} clan for giving {Context.Client.GetUser(135556895086870528).Username} the initial idea\n";
            message = message +"\nI also took heavy inspiration from a fellow bot, Blargbot, who was created by Stupid Cat (thats his username, not me being mean :3)\n" +
                               "You can find out more about blargbot on its website: <https://blargbot.xyz/>";
            await ReplyAsync(message, ReplyType.Info);
        }
    }
}
