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
        Task ParseAndExecute(IUserMessage message);
        CommandInfo? Search(string command, out int commandLength);
    }
}
