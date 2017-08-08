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
            public static class LanguageText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "LANGUAGE_";

                public const string MESSAGE_ADDITIONAL = BASE_PATH + nameof(MESSAGE_ADDITIONAL);

                public const string EMBED_DESCRIPTION = BASE_PATH + nameof(EMBED_DESCRIPTION);
                public const string COVERAGE = BASE_PATH + nameof(COVERAGE);
                public const string EXPORTED = BASE_PATH + nameof(EXPORTED);
                public const string ATTACHMENT_MISSING = BASE_PATH + nameof(ATTACHMENT_MISSING);
                public const string ATTACHMENT_EMPTY = BASE_PATH + nameof(ATTACHMENT_EMPTY);
                public const string IMPORTED = BASE_PATH + nameof(IMPORTED);
                public const string RELOADED = BASE_PATH + nameof(RELOADED);
                public const string CHANGED = BASE_PATH + nameof(CHANGED);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { EMBED_DESCRIPTION, "Here are all the supported languages by me!" },
                        { COVERAGE, "{0}% Coverage" },
                        { EXPORTED, "Here is the exported json for the {0} locale" },
                        { ATTACHMENT_MISSING, "You must supply an attachment in order to import a locale" },
                        { ATTACHMENT_EMPTY, "The supplied attachment was empty" },
                        { IMPORTED, "Sucessfully imported the {0} locale" },
                        { RELOADED, "Successfully reloaded the language files" },
                        { CHANGED, "Set your language to {0}" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
