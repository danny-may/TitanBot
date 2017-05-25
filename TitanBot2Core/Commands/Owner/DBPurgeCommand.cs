using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    public class DBPurgeCommand : Command
    {
        public DBPurgeCommand(CmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => PurgeAsync((string)a[0]))
                 .WithArgTypes(typeof(string));

            RequireOwner = true;
            Usage.Add("`{0} <table>` - Wipes the given table.");
            Description = "Wipes any database table clean";
        }

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
                    await ReplyAsync($"Unknown table `{table}`", ReplyType.Error);
                    return;
            }

            await ReplyAsync($"Dropped all data from the `{table}` table", ReplyType.Success);
        }
    }
}
