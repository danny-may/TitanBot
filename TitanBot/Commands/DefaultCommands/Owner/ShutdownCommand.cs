using System;
using System.Threading.Tasks;
using TitanBot.Util;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("SHUTDOWN_HELP_DESCRIPTION")]
    [RequireOwner]
    public class ShutdownCommand : Command
    {
        [Call]
        [Usage("SHUTDOWN_HELP_USAGE")]
        async Task ShutdownAsync(TimeSpan delay = default(TimeSpan))
        {
            if (delay > new TimeSpan(0, 0, 0))
                await ReplyAsync("SHUTDOWN_INTIME", ReplyType.Success, delay);
            await Task.Delay(delay);
            Bot.StopAsync().DontWait();
        }
    }
}
