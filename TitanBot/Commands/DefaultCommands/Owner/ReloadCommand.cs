using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.TextResource;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("RELOAD_HELP_DESCRIPTION")]
    [RequireOwner]
    class ReloadCommand : Command
    {
        ITextResourceManager TextManager { get; }

        public ReloadCommand(ITextResourceManager textManager)
        {
            TextManager = textManager;
        }

        [Usage("RELOAD_HELP_USAGE")]
        async Task ReloadAsync(Reloadable area = Reloadable.All)
        {
            switch (area)
            {
                case Reloadable.All:
                    break;
                case Reloadable.TextManager:
                    TextManager.Refresh();
                    break;
            }

            await ReplyAsync(TextResource.GetResource("RELOAD_COMPLETE", ReplyType.Success));
        }

        public enum Reloadable
        {
            All,
            TextManager,
        }
    }
}
