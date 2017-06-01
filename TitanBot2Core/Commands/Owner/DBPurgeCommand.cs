using System.Linq;
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
            var tables = await Context.Database.QueryAsync(conn => conn.GetCollectionNames());
            var target = tables.FirstOrDefault(t => t.ToLower() == table.ToLower());
            if (target != null)
            {
                await Context.Database.QueryAsync(conn => conn.DropCollection(target));
                await ReplyAsync($"Dropped all data from the `{target}` table", ReplyType.Success);
            }
            else
                await ReplyAsync($"The table `{table}` either does not exist or has no records.", ReplyType.Error);
        }
    }
}
