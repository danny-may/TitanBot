using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Owner
{
    [Description("Forces me to focus only on one channel or guild")]
    [RequireOwner]
    class FocusCommand : Command
    {
        [Call]
        [Usage("Sets the ID for me to focus on")]
        async Task FocusAsync(ulong? id = null)
        {
            var config = Configuration.Instance;
            config.FocusId = id;
            config.SaveJson();
            Configuration.Reload();

            await ReplyAsync($"Set focus to {id?.ToString() ?? "null"}", ReplyType.Success);
        }
    }
}
