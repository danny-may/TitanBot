using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot
{
    public static partial class TBLocalisation
    {
        public static partial class Commands
        {
            public static class InviteText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "INVITE_";

                public const string MESSAGE = BASE_PATH + nameof(MESSAGE);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { MESSAGE, "Want to invite me to your guild? Click this link!\n<https://discordapp.com/oauth2/authorize?client_id={0}&scope=bot&permissions={1}>" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
