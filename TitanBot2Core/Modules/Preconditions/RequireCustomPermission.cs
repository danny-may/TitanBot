using Discord.Commands;
using Discord;
using System;
using System.Threading.Tasks;

namespace TitanBot2.Modules.Preconditions
{
    public class RequireCustomPermission : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            
        }
    }
}
