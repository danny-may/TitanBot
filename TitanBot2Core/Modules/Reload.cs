using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Modules
{
    [Group("reload")]
    public class Reload : TitanBotModule
    {
        [Command("config")]
        public async Task ReloadConfigAsync()
        {
            Configuration.Reload();
            await ReplyAsync($"{Resources.Str.SuccessText} Reloaded Configuration");
        }

        [Command("resources")]
        [Alias("res")]
        public async Task ReloadResourcesAsync()
        {
            Resources.Reload();
            await ReplyAsync($"{Resources.Str.SuccessText} Reloaded Resources");
        }
    }
}
