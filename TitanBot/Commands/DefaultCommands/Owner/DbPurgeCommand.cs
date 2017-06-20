using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("Wipes any database table clean")]
    [RequireOwner]
    class DbPurgeCommand : Command
    {
        [Call]
        [Usage("Wipes the given table.")]
        async Task DropAsync(string table)
        {
            await Database.Drop(table);

            await ReplyAsync($"Attempted to drop all data from the `{table}` table", ReplyType.Success);
        }
    }
}
