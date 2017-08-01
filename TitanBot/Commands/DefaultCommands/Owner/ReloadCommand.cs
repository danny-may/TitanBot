using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Replying;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [RequireOwner]
    [Description(TitanBotResource.RELOAD_HELP_DESCRIPTION)]
    class ReloadCommand : Command
    {
        public Dictionary<string, Action> ReloadActions { get; } = new Dictionary<string, Action>();

        [Call]
        [Usage(TitanBotResource.RELOAD_HELP_USAGE)]
        async Task ReloadAsync(string area)
        {
            var key = ReloadActions.Keys.FirstOrDefault(k => k.ToUpper() == area.ToUpper());
            if (key == null)
                await ReplyAsync(TitanBotResource.RELOAD_AREA_NOTFOUND, ReplyType.Error, area);
            else
            {
                await ReplyAsync(TitanBotResource.RELOAD_SUCCESS, ReplyType.Success, key);
                ReloadActions[key]();
            }
        }

        [Call("List")]
        [Usage(TitanBotResource.RELOAD_HELP_USAGE_LIST)]
        async Task ListAsync()
        {
            await ReplyAsync(TitanBotResource.RELOAD_LIST, string.Join(", ", ReloadActions.Keys));
        }
    }
}
