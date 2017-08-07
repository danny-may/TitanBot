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
            var builder = new LocalisedEmbedBuilder();
            builder.AddField(f => f.WithRawName("test").WithValues(", ", new [] { "a", "b", "c" }));
            await ReplyAsync(builder);
        }
    }
}
