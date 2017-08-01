using System.Threading.Tasks;
using TitanBot.Replying;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(TitanBotResource.DBPURGE_HELP_DESCRIPTION)]
    [RequireOwner]
    public class DbPurgeCommand : Command
    {
        [Call]
        [Usage(TitanBotResource.DBPURGE_HELP_USAGE)]
        async Task DropAsync(string table)
        {
            await Database.Drop(table);

            await ReplyAsync(TitanBotResource.DBPURGE_SUCCESS, ReplyType.Success, table);
        }
    }
}
