using Discord.WebSocket;
using Titanbot.Command.Models;

namespace Titanbot.Command.Interfaces
{
    public interface IMessageSplitter
    {
        bool TryParseMessage(SocketUserMessage message, out string prefix, out string commandName, out string rawArg, out string[] args, out FlagValue[] flags);
    }
}