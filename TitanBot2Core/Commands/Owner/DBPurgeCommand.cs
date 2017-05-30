using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Owner
{
    [Description("Wipes any database table clean")]
    [RequireOwner]
    class DBPurgeCommand : Command
    {
        [Call]
        [Usage("Wipes the given table.")]
        async Task PurgeAsync(string table)
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
                    await ReplyAsync($"Unknown table `{table}`", ReplyType.Error);
                    return;
            }

            await ReplyAsync($"Dropped all data from the `{table}` table", ReplyType.Success);
        }
    }
}
