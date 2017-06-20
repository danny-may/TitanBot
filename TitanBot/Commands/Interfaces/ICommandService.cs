using Discord;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    public interface ICommandService
    {
        IReadOnlyList<CommandInfo> CommandList { get; }
        void Install(Assembly assembly);
        Task ParseAndExecute(IUserMessage message);
        CommandInfo? Search(string command, out int commandLength);
    }
}
