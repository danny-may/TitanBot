using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands.DefaultCommands.Owner
{
    [Description("Gets or sets the global prefix required to use commands")]
    [RequireOwner]
    class GlobalPrefix : Command
    {
        [Call]
        [DefaultPermission(0, "Show")]
        [Usage("Gets the current prefix")]
        async Task GetPrefixesAsync()
        {
            await ReplyAsync($"The current global prefix is `{GlobalSettings.DefaultPrefix}`", ReplyType.Info);
        }

        [Call]
        [DefaultPermission(8, "Set")]
        [RequireContext(ContextType.Guild)]
        [Usage("Sets the custom prefix")]
        async Task SetPrefixAsync(string newPrefix)
        {
            GlobalSettings.DefaultPrefix = newPrefix;
            await ReplyAsync($"The global prefix has been updated to `{GlobalSettings.DefaultPrefix}`", ReplyType.Success);
        }
    }
}
