using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot2.Commands.General
{
    [Description("DONATE_HELP_DESCRIPTION")]
    public class Donate : Command
    {
        [Call]
        [Usage("DONATE_HELP_USAGE")]
        async Task ShowPatreonAsync()
            => await ReplyAsync(TextResource.GetReplyType(ReplyType.Success) + 
                                    ":3 thank you for even considering this, heres the link to my patreon! <https://www.patreon.com/titansmasher>\n" + 
                                    TextResource.GetResource("DONATE_MESSAGE_ADDITIONAL"));
    }
}
