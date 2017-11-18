using System.Threading.Tasks;

namespace Titanbot.Command.DefaultCommands.General
{
    public class PingCommand : Command
    {
        [CommandFlag('c', "count", "Averages ping over period")]
        public int Count { get; set; }

        [Call]
        public async Task PingAsync(string arg1, int arg2 = 0)
        {
            await Task.Delay(10);
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}