using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.General
{
    [Description(Desc.ABOUT)]
    public class AboutCommand : Command
    {
        [Call]
        [Usage(Usage.ABOUT)]
        async Task ShowAbout()
        {
            await ReplyAsync(AboutText.MESSAGE, ReplyType.Info);
        }
    }
}
