using Discord;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Titanbot.Core.Command.Interfaces
{
    public interface ICommandService
    {
        event Func<LogMessage, Task> Log;

        void Install<TCommand>() where TCommand : Command;
        void Install(Type commandType);
        void Install(Type[] commands);
        void Install(Assembly assembly);

        Task HandleMessage(SocketMessage message);
    }
}