using Discord.WebSocket;

namespace Titanbot.Extensions.Settings.Interfaces
{
    public interface ISettingEditorContext
    {
        DiscordSocketClient Client { get; }
        SocketUserMessage Message { get; }
        ISocketMessageChannel Channel { get; }
        SocketUser User { get; }
    }
}