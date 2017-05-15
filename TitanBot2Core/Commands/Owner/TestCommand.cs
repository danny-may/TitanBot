using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    public class TestCommand : Command
    {
        public TestCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(ExecuteAsync)
                 .WithArgTypes(typeof(SocketGuildUser));
            RequireOwner = true;
            Description = "Does something, probably";
            Usage.Add("How should I know 0_o");
        }

        private async Task ExecuteAsync(object[] args)
        {
            var user = (SocketGuildUser)args[0];

            await TrySend(user.Id, "testc");
        }
    }
}
