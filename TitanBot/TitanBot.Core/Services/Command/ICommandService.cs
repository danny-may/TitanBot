using Discord;
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

        string DefaultPrefix { get; }

        void Install<T>() where T : class, ICommand;
        void Install(Type[] commandTypes);
        void Install(Assembly assembly);

        ICommandInfo[] Search(string command, out int commandLength);
        Task Handle(IUserMessage message);
        void AddBuildEvent<T>(Func<T, Task> handler) where T : ICommand;
        void RemoveBuildEvent<T>(Func<T, Task> handler) where T : ICommand;
    }
}