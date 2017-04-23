using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Handlers
{
    public class ChannelHandler : HandlerBase
    {

        public override async Task Install(TitanbotDependencies args)
        {
            await base.Install(args);

            Client.ChannelCreated += HandleCreatedAsync;
            Client.ChannelDestroyed += HandleDestroyedAsync;
            Client.ChannelUpdated += HandleUpdatedAsync;
            Client.RecipientAdded += HandleRecipientAddedAsync;
            Client.RecipientRemoved += HandleRecipientRemovedAsync;

            await TitanBot.Logger.Log(new LogEntry(LogType.Handler, "Installed successfully", "ChannelHandler"));
        }

        public override async Task Uninstall()
        {
            Client.ChannelCreated -= HandleCreatedAsync;
            Client.ChannelDestroyed -= HandleDestroyedAsync;
            Client.ChannelUpdated -= HandleUpdatedAsync;

            Client.RecipientAdded -= HandleRecipientAddedAsync;
            Client.RecipientRemoved -= HandleRecipientRemovedAsync;

            await TitanBot.Logger.Log(new LogEntry(LogType.Handler, "Uninstalled successfully", "ChannelHandler"));

            await base.Uninstall();
        }

        private async Task HandleCreatedAsync(SocketChannel channel)
        {

        }

        private async Task HandleDestroyedAsync(SocketChannel channel)
        {

        }

        private async Task HandleUpdatedAsync(SocketChannel oldChannel, SocketChannel newChannel)
        {

        }

        private async Task HandleRecipientAddedAsync(SocketGroupUser user)
        {

        }

        private async Task HandleRecipientRemovedAsync(SocketGroupUser user)
        {

        }
    }
}
