using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Replying;

namespace TitanBot.Commands.General
{
    [Description(TitanBotResource.ABOUT_HELP_DESCRIPTION)]
    public class AboutCommand : Command
    {
        private readonly Dictionary<ulong, string> _specialThanks = new Dictionary<ulong, string>();

        [Call]
        [Usage(TitanBotResource.ABOUT_HELP_USAGE)]
        async Task ShowAbout()
        {
            await ReplyAsync(TitanBotResource.ABOUT_MESSAGE, ReplyType.Info);
        }
    }
}
