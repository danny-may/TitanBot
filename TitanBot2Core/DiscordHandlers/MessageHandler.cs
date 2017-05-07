using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.DiscordHandlers
{
    public class MessageHandler : HandlerBase
    {
        private TitanBotCommandService _cmds;

        public override async Task Install(TitanbotDependencies args)
        {
            await base.Install(args);

            _cmds = new TitanBotCommandService();

            await _cmds.Install(Assembly.GetExecutingAssembly());

            Client.MessageReceived += HandleRecieveAsync;
            Client.MessageDeleted += HandleDeleteAsync;
            Client.MessageUpdated += HandleUpdateAsync;
            Client.ReactionAdded += HandleReactionAddAsync;
            Client.ReactionRemoved += HandleReactionRemoveAsync;
            Client.ReactionsCleared += HandleReactionClearedAsync;

            await TitanBot.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Info, "Installed successfully", "Message"));
        }

        public override async Task Uninstall()
        {
            Client.MessageReceived -= HandleRecieveAsync;
            Client.MessageDeleted -= HandleDeleteAsync;
            Client.MessageUpdated -= HandleUpdateAsync;
            Client.ReactionAdded -= HandleReactionAddAsync;
            Client.ReactionRemoved -= HandleReactionRemoveAsync;
            Client.ReactionsCleared -= HandleReactionClearedAsync;

            await TitanBot.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Info, "Uninstalled successfully", "Message"));
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
            var msg = s as SocketUserMessage;
            if (msg == null || (msg.Author.IsBot && msg.Author.Id != 134133271750639616))
                return;

            var context = new TitanbotCmdContext(Dependencies, msg);
            
            var argPos = await context.CheckCommand();
            if (argPos != null)
            {
                await _cmds.ExecuteAsync(context, argPos.Value);
            }
        }
    }
}
