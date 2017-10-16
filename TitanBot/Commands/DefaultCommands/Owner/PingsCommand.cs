using System.Threading.Tasks;
using TitanBot.Formatting;
using TitanBot.Replying;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [RequireOwner]
    public class PingsCommand : Command
    {
        [Call("Toggle")]
        public Task ToggleAsync()
            => SetAsync(!DisablePings);

        [Call]
        public async Task SetAsync(bool allow)
        {
            DisablePings = !allow;
            await ReplyAsync(new RawString("Pings have been " + (DisablePings ? "enabled" : "disabled"), ReplyType.Success));
        }
    }
}