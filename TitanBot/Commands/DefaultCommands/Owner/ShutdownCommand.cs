using System;
using System.Threading.Tasks;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(Desc.SHUTDOWN)]
    [RequireOwner]
    public class ShutdownCommand : Command
    {
        [Call]
        [Usage(Usage.SHUTDOWN)]
        async Task ShutdownAsync(TimeSpan delay = default(TimeSpan))
        {
            if (delay > new TimeSpan(0, 0, 0))
                await ReplyAsync(ShutdownText.INTIME, ReplyType.Success, delay);
            await Task.Delay(delay);
            Bot.StopAsync().DontWait();
        }
    }
}
