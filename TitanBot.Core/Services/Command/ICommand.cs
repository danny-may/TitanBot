using TitanBot.Core.Models.Contexts;

namespace TitanBot.Core.Services.Command
{
    public interface ICommand
    {
        void Install(ICommandContext context);
    }
}