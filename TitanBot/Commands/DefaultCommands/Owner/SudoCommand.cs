using Discord;
using System.Threading.Tasks;
using TitanBot.Commands.Models;
using TitanBot.Dependencies;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Commands;
using static TitanBot.TBLocalisation.Help;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(Desc.SUDOCOMMAND)]
    [RequireOwner]
    public class SudoCommand : Command
    {
        private IDependencyFactory Factory { get; }

        public SudoCommand(IDependencyFactory factory)
        {
            Factory = factory;
        }

        [Call]
        [Usage(Usage.SUDOCOMMAND)]
        async Task SudoAsync(IUser user, [Dense]string command)
        {
            var spoofMessage = new SudoMessage(Message)
            {
                Author = user,
                Content = command
            };
            await ReplyAsync(SudoText.SUCCESS, ReplyType.Success, command, user);
            CommandService.AddToProcessQueue(spoofMessage).DontWait();
        }
    }
}
