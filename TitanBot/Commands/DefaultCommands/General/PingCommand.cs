using System.Threading.Tasks;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description(Desc.PING)]
    public class PingCommand : Command
    {
        protected override bool ShowTyping => false;
        protected override bool WaitForTyping => false;

        [Call]
        [Usage(Usage.PING)]
        async Task SendPongAsync()
        {
            var msg = await ReplyAsync(PingText.INITIAL, ReplyType.Success, Client.Latency);
            Modify(msg).ChangeMessage(PingText.VERIFY, ReplyType.Success, (msg.Timestamp - Message.Timestamp).TotalMilliseconds).Modify();
        }
    }
}
