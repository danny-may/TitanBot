using Discord;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services;
using TitanBot2.Services.CommandService;

namespace TitanBot2.DiscordHandlers
{
    public class MessageHandler : HandlerBase
    {
        private BotCommandService _cmds;

        private DateTime _lastWave = DateTime.Now;

        public override async Task Install(BotDependencies args)
        {
            await base.Install(args);

            _cmds = new BotCommandService();

            await _cmds.Install(Assembly.GetExecutingAssembly());

            Client.MessageReceived += HandleRecieveAsync;
            Client.MessageDeleted += HandleDeleteAsync;
            Client.MessageUpdated += HandleUpdateAsync;
            Client.ReactionAdded += HandleReactionAddAsync;
            Client.ReactionRemoved += HandleReactionRemoveAsync;
            Client.ReactionsCleared += HandleReactionClearedAsync;

            await BotClient.Logger.Log(new BotLog(LogType.Handler, LogSeverity.Info, "Installed successfully", "Message"));
        }

        public override async Task Uninstall()
        {
            Client.MessageReceived -= HandleRecieveAsync;
            Client.MessageDeleted -= HandleDeleteAsync;
            Client.MessageUpdated -= HandleUpdateAsync;
            Client.ReactionAdded -= HandleReactionAddAsync;
            Client.ReactionRemoved -= HandleReactionRemoveAsync;
            Client.ReactionsCleared -= HandleReactionClearedAsync;

            await BotClient.Logger.Log(new BotLog(LogType.Handler, LogSeverity.Info, "Uninstalled successfully", "Message"));
            await base.Uninstall();
        }

        private async Task HandleRecieveAsync(SocketMessage msg)
        {
            await CheckAndRunCommands(msg);
        }

        private async Task HandleDeleteAsync(Cacheable<IMessage, ulong> cachedMsg, ISocketMessageChannel channel)
        {

        }

        private async Task HandleUpdateAsync(Cacheable<IMessage, ulong> cachedMsg, SocketMessage msg, ISocketMessageChannel channel)
        {

        }

        private async Task HandleReactionAddAsync(Cacheable<IUserMessage, ulong> cachedMsg, ISocketMessageChannel channel, SocketReaction reaction)
        {

        }

        private async Task HandleReactionRemoveAsync(Cacheable<IUserMessage, ulong> cachedMsg, ISocketMessageChannel channel, SocketReaction reaction)
        {

        }

        private async Task HandleReactionClearedAsync(Cacheable<IUserMessage, ulong> cachedMsg, ISocketMessageChannel channel)
        {

        }

        private async Task CheckAndRunCommands(SocketMessage s)
        {
            if (Configuration.Instance.FocusId != null && Configuration.Instance.FocusId != s.Channel.Id && Configuration.Instance.FocusId != (s.Channel as SocketGuildChannel).Guild.Id)
                return;

            var msg = s as SocketUserMessage;
            if (msg == null || (msg.Author.IsBot && msg.Author.Id != 134133271750639616))
                return;

            if ((msg.Content.StartsWith("👋") || msg.Content.StartsWith(":wave:")) && DateTime.Now.AddSeconds(-20) > _lastWave)
            {
                _lastWave = DateTime.Now;
                await msg.Channel.SendMessageSafeAsync("👋");
            }

            var context = new CmdContext(Dependencies, _cmds, msg);
            
            if (context.IsCommand())
            {
                _cmds.CheckCommandAsync(context);
            }
        }
    }
}
