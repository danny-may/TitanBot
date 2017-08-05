using Discord;
using System.Threading.Tasks;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [RequireOwner]
    class TestCommand : Command
    {
        [Call]
        async Task TestAsync()
        {
            await Task.Delay(5000);
            await ReplyAsync(new string('a', 3000));
            var x = new EmbedBuilder();
        }
    }
}
