using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Models;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Clan
{
    public class ApplicationsCommand : Command
    {
        public ApplicationsCommand()
        {
            //RequiredContexts = ContextType.Guild;
            //RequireOwner = true;
        }
    }
}
