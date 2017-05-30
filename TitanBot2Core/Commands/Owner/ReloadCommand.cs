using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Owner
{
    [Description("Reloads various settings")]
    [RequireOwner]
    class ReloadCommand : Command
    {
        [Call]
        [Usage("Resets some caches and settings")]
        async Task ReloadAsync()
        {
            Configuration.Reload();
            Context.Dependencies.SuggestionChannelID = Configuration.Instance.SuggestChannel;
            Context.Dependencies.BugChannelID = Configuration.Instance.BugChannel;
            Context.WebClient.WipeCache();

            await ReplyAsync("Cache and configuration reset", ReplyType.Success);
        }

    }
}
