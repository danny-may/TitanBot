using Discord;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    public interface ICommandService
    {
        IReadOnlyList<CommandInfo> CommandList { get; }
        void Install(Assembly assembly);
        void Install(Type[] types);
        Task AddToProcessQueue(IUserMessage message);
        CommandInfo? Search(string command, out int commandLength);
        void AddBuildEvent<T>(Action<T> handler) where T : Command;
        List<Action<Command>> GetBuildEvents(Type commandType);
        List<Action<T>> GetBuildEvents<T>() where T : Command;
    }
}
