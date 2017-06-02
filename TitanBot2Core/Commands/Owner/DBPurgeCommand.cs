using LiteDB;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Extensions;
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
        async Task PurgeAsync(string table, string column = null)
        {
            var tables = await Context.Database.QueryAsync(conn => conn.GetCollectionNames());
            var target = tables.FirstOrDefault(t => t.ToLower() == table.ToLower());
            if (target == null)
            {
                await ReplyAsync($"The table `{table}` either does not exist or has no records.", ReplyType.Error);
                return;
            }
            if (column == null)
            {
                await Context.Database.QueryAsync(conn => conn.DropCollection(target));
                await ReplyAsync($"Dropped all data from the `{target}` table", ReplyType.Success);
            }
            else
            {
                var data = await Context.Database.QueryAsync(conn => conn.GetCollection(target).FindAll());
                var key = data.SelectMany(r => r.Keys).Distinct().FirstOrDefault(k => k.ToLower() == column.ToLower());
                if (key == null)
                {
                    await ReplyAsync($"The `{target}` table does not contain the column `{column}`", ReplyType.Error);
                    return;
                }
                var rows = data.Cast<BsonDocument>().ToList();
                foreach (var row in rows)
                    row.Remove(key);
                await Context.Database.QueryAsync(conn => conn.GetCollection(target).Upsert(rows));
                await ReplyAsync($"Data purged from {target}.{key}", ReplyType.Success);
            }
                
        }
    }
}
