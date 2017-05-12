using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    public class ReloadCommand : Command
    {
        public ReloadCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            RequireOwner = true;
        }


    }
}
