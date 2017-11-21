using Discord.WebSocket;
using System;
using Titanbot.Extensions.Settings.Interfaces;

namespace Titanbot.Extensions.Settings
{
    internal class SettingEditorContext : ISettingEditorContext
    {
        public DiscordSocketClient Client { get; }
        public SocketUserMessage Message { get; }
        public ISocketMessageChannel Channel => Message.Channel;
        public SocketUser User => Message.Author;

        public SettingEditorContext(DiscordSocketClient client, SocketUserMessage message)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}