using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    public class ReloadCommand : Command
    {
        public ReloadCommand(CmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ReloadAsync());
            RequireOwner = true;
            Usage.Add("`{0}` - Reloads my bots configuration file and wipes the stored webcache");
            Description = "Reloads various settings";
        }

        private async Task ReloadAsync()
        {
            Configuration.Reload();
            Context.Dependencies.SuggestionChannelID = Configuration.Instance.SuggestChannel;
            Context.Dependencies.BugChannelID = Configuration.Instance.BugChannel;
            Context.WebClient.WipeCache();

            await ReplyAsync("Cache and configuration reset", ReplyType.Success);
        }

    }
}
