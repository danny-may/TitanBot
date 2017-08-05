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
            public static class ExceptionText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "ExceptionText";

                public const string NOTFOUND = BASE_PATH + nameof(NOTFOUND);
                public const string MESSAGE = BASE_PATH + nameof(MESSAGE);
                public const string CHANNEL = BASE_PATH + nameof(CHANNEL);
                public const string GUILD = BASE_PATH + nameof(GUILD);
                public const string FULLMESSAGE = BASE_PATH + nameof(FULLMESSAGE);
                public const string USER = BASE_PATH + nameof(USER);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { NOTFOUND, "Exception {0} has not yet occured!" },
                        { MESSAGE, "Message" },
                        { CHANNEL, "Channel:\n{0} ({1})" },
                        { GUILD, "Guild" },
                        { FULLMESSAGE, "Here is everything that was logged for exception {0}" },
                        { USER, "USER:\n{0}" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
