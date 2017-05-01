using Discord.Commands;
using System;
using System.Threading.Tasks;
using TitanBot2.TypeReaders;

namespace TitanBot2.Modules.Owner
{
    [Name("Owner")]
    [RequireOwner]
    public partial class OwnerModule : TitanBotModule
    {
    }
}
