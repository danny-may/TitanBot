using Discord.WebSocket;
using Titanbot.Commands.Models;

namespace Titanbot.Commands.Interfaces
{
    public interface IMessageSplitter
    {
        bool TryParseMessage(SocketUserMessage message, out string prefix, out string commandName, out string rawArg, out string[] args, out FlagValue[] flags);
    }
}