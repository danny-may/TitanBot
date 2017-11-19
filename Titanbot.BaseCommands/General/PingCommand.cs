using System.Threading.Tasks;
using Titanbot.Commands;

namespace Titanbot.BaseCommands.General
{
    [Description("Ping Description")]
    public class PingCommand : CommandBase
    {
        [Call]
        [Usage("Ping usage")]
        [HideTyping]
        public async Task SendPongAsync()
        {
            //var msg = await ReplyAsync(PingText.INITIAL, ReplyType.Success, Client.Latency);
            //Modify(msg).ChangeMessage(PingText.VERIFY, ReplyType.Success, (msg.Timestamp - Message.Timestamp).TotalMilliseconds).Modify();
        }

        #region IDisposable

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }

        #endregion IDisposable
    }
}