using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [RequireOwner]
    class TestCommand : Command
    {
        protected override int DelayMessageMs => 1000000000;

        [Call]
        async Task TestAsync()
        {
            await ReplyAsync(new string('a', 3000));
        }
    }
}
