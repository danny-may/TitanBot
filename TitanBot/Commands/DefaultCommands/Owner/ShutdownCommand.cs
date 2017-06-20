using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("Shuts me down")]
    [RequireOwner]
    class ShutdownCommand : Command
    {
        [Call]
        async Task ShutdownAsync()
        {
            Bot.StopAsync();
        }
    }
}
