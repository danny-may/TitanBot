using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Modules.Owner
{
    public partial class OwnerModule
    {
        [Group("DbPurge")]
        [Summary("Deletes all data from a given table")]
        public class DBPurge : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            public async Task PurgeAsync(string table)
            {
                switch (table.ToLower())
                {
                    case "timer":
                        await Context.Database.Timers.Drop();
                        break;
                    case "cmdperm":
                        await Context.Database.CmdPerms.Drop();
                        break;
                    case "guild":
                        await Context.Database.Guilds.Drop();
                        break;
                    case "excuse":
                        await Context.Database.Excuses.Drop();
                        break;
                    default:
                        await ReplyAsync($"{Res.Str.ErrorText} Unknown table `{table}`");
                        return;
                }

                await ReplyAsync($"{Res.Str.SuccessText} Dropped all data from the `{table}` table");
            }
        }
    }
}
