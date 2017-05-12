using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Clan
{
    public class RegisterCommand : Command
    {
        public RegisterCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            RequiredContexts = Discord.Commands.ContextType.Guild;
            RequireOwner = true;
        }

        private async Task RegisterAsync(int maxStage, string message)
        {
            var current = await Context.Database.Registrations.Get(r => r.GuildId == Context.Guild.Id && r.UserId == Context.User.Id);
            
        }
    }
}
