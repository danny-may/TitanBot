using Discord.WebSocket;
using Titanbot.Core.Command.Models;

namespace Titanbot.Core.Command.Interfaces
{
    public interface IMessageSplitter
    {
        bool TryParseMessage(SocketUserMessage message, out string prefix, out string commandName, out string[] args, out CommandFlag[] flags);
    }
}