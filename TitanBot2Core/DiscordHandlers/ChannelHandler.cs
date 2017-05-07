using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.DiscordHandlers
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

            await TitanBot.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Info, "Installed successfully", "Channel"));
        }

        public override async Task Uninstall()
        {
            Client.ChannelCreated -= HandleCreatedAsync;
            Client.ChannelDestroyed -= HandleDestroyedAsync;
            Client.ChannelUpdated -= HandleUpdatedAsync;

            Client.RecipientAdded -= HandleRecipientAddedAsync;
            Client.RecipientRemoved -= HandleRecipientRemovedAsync;

            await TitanBot.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Info, "Uninstalled successfully", "Channel"));

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
