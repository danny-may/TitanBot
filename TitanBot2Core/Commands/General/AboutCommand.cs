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
        }

        public async Task ShowAbout()
        {
            var message = $@"'Ello there :wave:, Im Titanbot! Im an open source discord bot dedicated to TT2 written in C# by Titansmasher.
I can do a variety of things, all of which are better documented in my `{Context.Prefix}help` command!

Im always going to be taking suggestions, which can either be DM'd to Titansmasher (He has a terrible memory though) Or sent in the #suggestions channel on my support server!

Public repository: <https://github.com/Titansmasher/TitanBot2>
Support server: <https://discord.gg/abQPtHu>

I also took heavy inspiration from a fellow bot, Blargbot, who was created by Stupid Cat. :heart:
You can find out more about blargbot on its website: <https://blargbot.xyz/>";
            await ReplyAsync(message, ReplyType.Info);
        }
    }
}
