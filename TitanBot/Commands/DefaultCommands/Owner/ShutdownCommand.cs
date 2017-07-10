using System.Threading.Tasks;
using TitanBot.Util;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("Shuts me down")]
    [RequireOwner]
    public class ShutdownCommand : Command
    {
        [Call]
        Task ShutdownAsync()
        {
            Bot.StopAsync().DontWait();
            return Task.CompletedTask;
        }
    }
}
