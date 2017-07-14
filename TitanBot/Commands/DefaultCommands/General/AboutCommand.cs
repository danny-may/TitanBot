using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot2.Commands.General
{
    [Description("ABOUT_HELP_DESCRIPTION")]
    public class AboutCommand : Command
    {
        private readonly Dictionary<ulong, string> _specialThanks = new Dictionary<ulong, string>();

        [Call]
        [Usage("ABOUT_HELP_USAGE")]
        async Task ShowAbout()
        {
            await ReplyAsync("ABOUT_MESSAGE", ReplyType.Info);
        }
    }
}
