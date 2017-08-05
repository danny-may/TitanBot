using System.Threading.Tasks;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.General
{
    [Description(Desc.DONATE)]
    public class Donate : Command
    {
        [Call]
        [Usage(Usage.DONATE)]
        async Task ShowPatreonAsync()
            => await ReplyAsync(TextResource.GetReplyType(ReplyType.Success) + 
                                ":3 thank you for even considering this, heres the link to my patreon! <https://www.patreon.com/titansmasher>\n" + 
                                TextResource.GetResource(DonateText.MESSAGE_ADDITIONAL));
    }
}
