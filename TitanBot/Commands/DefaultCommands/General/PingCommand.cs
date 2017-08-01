using System.Threading.Tasks;
using TitanBot.Replying;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description(TitanBotResource.PING_HELP_DESCRIPTION)]
    public class PingCommand : Command
    {
        [Call]
        [Usage(TitanBotResource.PING_HELP_USAGE)]
        async Task SendPongAsync()
        {
            var msg = await ReplyAsync(TitanBotResource.PING_INITIAL, ReplyType.Success, Client.Latency);
            Modify(msg).ChangeMessage(TitanBotResource.PING_VERIFY, ReplyType.Success, (msg.Timestamp - Message.Timestamp).TotalMilliseconds).Modify();
        }
    }
}
