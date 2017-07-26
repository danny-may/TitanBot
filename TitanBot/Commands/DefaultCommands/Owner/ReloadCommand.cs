using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [RequireOwner]
    [Description("RELOAD_HELP_DESCRIPTION")]
    class ReloadCommand : Command
    {
        public static Dictionary<string, Action> ReloadActions { get; } = new Dictionary<string, Action>();

        [Call]
        [Usage("RELOAD_HELP_USAGE")]
        async Task ReloadAsync(string area)
        {
            var key = ReloadActions.Keys.FirstOrDefault(k => k.ToUpper() == area.ToUpper());
            if (key == null)
                await ReplyAsync("RELOAD_AREA_NOTFOUND", ReplyType.Error, area);
            else
            {
                await ReplyAsync("RELOAD_SUCCESS", ReplyType.Success, key);
                ReloadActions[key]();
            }
        }

        [Call("List")]
        [Usage("RELOAD_HELP_USAGE_LIST")]
        async Task ListAsync()
        {
            await ReplyAsync("RELOAD_LIST", string.Join(", ", ReloadActions.Keys));
        }
    }
}
