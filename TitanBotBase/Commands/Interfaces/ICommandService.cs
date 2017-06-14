using Discord;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace TitanBotBase.Commands
{
    public interface ICommandService
    {
        IReadOnlyList<CommandInfo> Commands { get; }
        void Install(Assembly assembly);
        Task ParseAndExecute(IUserMessage message);
        CommandInfo? Search(string command, out int commandLength);
    }
}
