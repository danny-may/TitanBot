using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.General
{
    [Description("You.. youre thinking of donating? :open_mouth: This command will give you a link to my patreon page")]
    class Donate : Command
    {
        [Call]
        [Usage("Gives you the link you awesome person :heart:")]
        async Task ShowPatreonAsync()
            => await ReplyAsync(":3 thank you for even considering this, heres the link to my page! <https://www.patreon.com/titansmasher>", ReplyType.Info);
    }
}
