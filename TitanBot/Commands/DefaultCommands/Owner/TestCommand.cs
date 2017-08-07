using Discord;
using System.Threading.Tasks;
using TitanBot.Formatting;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [RequireOwner]
    class TestCommand : Command
    {
        [Call]
        async Task TestAsync()
        {
            var builder = new LocalisedEmbedBuilder();
            builder.AddField(f => f.WithRawName("test").WithValue(tr => tr.ToString()));
            await ReplyAsync(builder);
        }
    }
}
