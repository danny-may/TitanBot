using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [RequireOwner]
    [Description(Desc.RELOAD)]
    class ReloadCommand : Command
    {
        public Dictionary<string, Action> ReloadActions { get; } = new Dictionary<string, Action>();

        [Call]
        [Usage(Usage.RELOAD)]
        async Task ReloadAsync(string area)
        {
            var key = ReloadActions.Keys.FirstOrDefault(k => k.ToUpper() == area.ToUpper());
            if (key == null)
                await ReplyAsync(ReloadText.AREA_NOTFOUND, ReplyType.Error, area);
            else
            {
                await ReplyAsync(ReloadText.SUCCESS, ReplyType.Success, key);
                ReloadActions[key]();
            }
        }

        [Call("List")]
        [Usage(Usage.RELOAD_LIST)]
        async Task ListAsync()
        {
            await ReplyAsync(ReloadText.LIST, string.Join(", ", ReloadActions.Keys));
        }
    }
}
