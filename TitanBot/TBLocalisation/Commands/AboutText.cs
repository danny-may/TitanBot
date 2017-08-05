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
            public static class AboutText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "ABOUT_";

                public const string MESSAGE = BASE_PATH + nameof(MESSAGE);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { MESSAGE, "'Ello there :wave:, Im Titanbot! Im an open source discord bot library written in C# by Titansmasher, with a fair bit of help from Mildan too!.\n" +
                                   "This is unfortunately just my base functionality, however there are other bots that run using this bot as its core library.\n" +
                                   "If you wish to use this library to make your own bot, feel free to do so! You can find me on github here: <https://github.com/Titansmasher/TitanBot>\n\n" +
                                   "_If you are seeing this on a fully fledged bot, please contact the owner of said bot because they appear to have not supplied their own about message_" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
