using System.Threading.Tasks;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(Desc.DBPURGE)]
    [RequireOwner]
    public class DbPurgeCommand : Command
    {
        [Call]
        [Usage(Usage.DBPURGE)]
        async Task DropAsync(string table)
        {
            await Database.Drop(table);

            await ReplyAsync(DbPurgeText.SUCCESS, ReplyType.Success, table);
        }
    }
}
