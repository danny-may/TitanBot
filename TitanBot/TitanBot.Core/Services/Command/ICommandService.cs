using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Core.Services.Command.Models;

namespace TitanBot.Core.Services.Command
{
    public interface ICommandService
    {
        IReadOnlyList<ICommandInfo> CommandHistory { get; }
        IReadOnlyList<ICommandInfo> InstalledCommands { get; }

        string DefaultPrefix { get; }

        void Install<T>() where T : class, ICommand;
        void Install(params Type[] commandTypes);
        void Install(Assembly assembly);

        ICommandInfo[] Search(string command, out int commandLength);
        Task Handle(SocketUserMessage message);
        void AddBuildEvent<T>(Func<T, Task> handler) where T : ICommand;
        void RemoveBuildEvent<T>(Func<T, Task> handler) where T : ICommand;
    }
}