using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Commands.Responses;

namespace TitanBotBase.Commands
{
    public interface ICommandService
    {
        IReadOnlyList<CommandInfo> Commands { get; }
        string DefaultPrefix { get; set; }
        void Install(Assembly assembly);
        Task ParseAndExecute(IUserMessage message);
        CommandInfo? Search(string command, out int commandLength);
    }
}
