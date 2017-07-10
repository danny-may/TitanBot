using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot2.Commands.General
{
    [Description("You.. youre thinking of donating? :open_mouth: This command will give you a link to my patreon page")]
    public class Donate : Command
    {
        [Call]
        [Usage("Gives you the link you awesome person :heart:")]
        async Task ShowPatreonAsync()
            => await ReplyAsync(":3 thank you for even considering this, heres the link to my page! <https://www.patreon.com/titansmasher>", ReplyType.Info);
    }
}
