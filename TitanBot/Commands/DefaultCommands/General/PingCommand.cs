using System.Threading.Tasks;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description("PING_HELP_DESCRIPTION")]
    public class PingCommand : Command
    {
        [Call]
        [Usage("PING_HELP_USAGE")]
        async Task SendPongAsync()
        {
            var msg = await ReplyAsync("PING_INITIAL", ReplyType.Success, Client.Latency);
            Modify(msg).ChangeMessage("PING_VERIFY", ReplyType.Success, (msg.Timestamp - Message.Timestamp).TotalMilliseconds).Modify();
        }
    }
}
