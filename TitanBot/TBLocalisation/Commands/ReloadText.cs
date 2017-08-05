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
            public static class ReloadText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "RELOAD_";

                public const string AREA_NOTFOUND = BASE_PATH + nameof(AREA_NOTFOUND);
                public const string SUCCESS = BASE_PATH + nameof(SUCCESS);
                public const string LIST = BASE_PATH + nameof(LIST);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { AREA_NOTFOUND, "The area `{0}` does not exist" },
                        { SUCCESS, "Reloading {0}" },
                        { LIST, "Here are all the areas that can be reloaded:\n{0}" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
