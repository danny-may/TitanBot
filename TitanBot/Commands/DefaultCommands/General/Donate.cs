using System.Threading.Tasks;
using TitanBot.Replying;

namespace TitanBot.Commands.General
{
    [Description(TitanBotResource.DONATE_HELP_DESCRIPTION)]
    public class Donate : Command
    {
        [Call]
        [Usage(TitanBotResource.DONATE_HELP_USAGE)]
        async Task ShowPatreonAsync()
            => await ReplyAsync(TextResource.GetReplyType(ReplyType.Success) + 
                                ":3 thank you for even considering this, heres the link to my patreon! <https://www.patreon.com/titansmasher>\n" + 
                                TextResource.GetResource(TitanBotResource.DONATE_MESSAGE_ADDITIONAL));
    }
}
