using System.Collections.Generic;
using System.Threading.Tasks;

namespace TitanBot.Commands.General
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
