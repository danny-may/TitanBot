using Discord.WebSocket;
using Titanbot.Core.Command.Models;

namespace Titanbot.Core.Command.Interfaces
{
    public interface IMessageSplitter
    {
        bool TryParseMessage(SocketUserMessage message, out string prefix, out string commandName, out string rawArg, out string[] args, out FlagValue[] flags);
    }
}