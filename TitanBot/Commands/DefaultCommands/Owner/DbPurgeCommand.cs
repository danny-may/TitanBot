using System.Threading.Tasks;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("DBPURGE_HELP_DESCRIPTION")]
    [RequireOwner]
    public class DbPurgeCommand : Command
    {
        [Call]
        [Usage("DBPURGE_HELP_USAGE")]
        async Task DropAsync(string table)
        {
            await Database.Drop(table);

            await ReplyAsync("DBPURGE_SUCCESS", ReplyType.Success, table);
        }
    }
}
