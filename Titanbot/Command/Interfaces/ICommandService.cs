using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Titanbot.Command.Models;

namespace Titanbot.Command.Interfaces
{
    public interface ICommandService
    {
        ICommandService Install<TCommand>()
            where TCommand : Command;
        ICommandService Install(Type commandType);
        ICommandService Install(Type[] commands);
        ICommandService Install(Assembly assembly);

        ICommandService Uninstall<TCommand>()
            where TCommand : Command;
        ICommandService Uninstall(Type commandType);
        ICommandService Uninstall(Type[] commands);
        ICommandService Uninstall(Assembly assembly);

        ICommandService RegisterService<TService>()
            where TService : class;
        ICommandService RegisterService<TService>(TService service)
            where TService : class;
        ICommandService RegisterService<TService, TImplimentation>()
            where TImplimentation : class, TService
            where TService : class;
        ICommandService RegisterService<TService, TImplimentation>(TImplimentation service)
            where TImplimentation : class, TService
            where TService : class;

        Task HandleMessage(SocketMessage message);
        CommandInfo Search(string commandName);
    }
}