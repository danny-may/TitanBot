using System.Threading.Tasks;
using TitanBot.Settings;
using static TitanBot.TBLocalisation.Help;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(Desc.LOGOUT)]
    [RequireOwner]
    class LogoutCommand : Command
    {
        [Call]
        [Usage(Usage.LOGOUT)]
        Task LogoutAsync()
        {
            GlobalSettings.Edit<GeneralGlobalSetting>(s => s.Token = null);
            Bot.StopAsync().DontWait();
            return Task.CompletedTask;
        }
    }
}
