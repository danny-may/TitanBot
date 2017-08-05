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
            public static class SettingText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "SETTING_";

                public const string FOOTERTEXT = BASE_PATH + nameof(FOOTERTEXT);
                public const string TITLE_NOGROUP = BASE_PATH + nameof(TITLE_NOGROUP);
                public const string DESCRIPTION_NOSETTINGS = BASE_PATH + nameof(DESCRIPTION_NOSETTINGS);
                public const string INVALIDGROUP = BASE_PATH + nameof(INVALIDGROUP);
                public const string TITLE_GROUP = BASE_PATH + nameof(TITLE_GROUP);
                public const string NOTSET = BASE_PATH + nameof(NOTSET);
                public const string KEY_NOTFOUND = BASE_PATH + nameof(KEY_NOTFOUND);
                public const string UNABLE_TOGGLE = BASE_PATH + nameof(UNABLE_TOGGLE);
                public const string VALUE_INVALID = BASE_PATH + nameof(VALUE_INVALID);
                public const string VALUE_CHANGED_TITLE = BASE_PATH + nameof(VALUE_CHANGED_TITLE);
                public const string VALUE_OLD = BASE_PATH + nameof(VALUE_OLD);
                public const string VALUE_NEW = BASE_PATH + nameof(VALUE_NEW);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { FOOTERTEXT, "{0} | Settings" },
                        { TITLE_NOGROUP, "Please select a setting group from the following:" },
                        { DESCRIPTION_NOSETTINGS, "No settings groups available!" },
                        { INVALIDGROUP, "`{0}`isnt a valid setting group!" },
                        { TITLE_GROUP, "Here are all the settings for the group `{0}`" },
                        { NOTSET, "Not Set" },
                        { KEY_NOTFOUND, "Could not find the `{0}` setting" },
                        { UNABLE_TOGGLE, "You cannot toggle the `{0}` setting" },
                        { VALUE_INVALID, "`{1}` is not a valid value for the setting {0}" },
                        { VALUE_CHANGED_TITLE, "{0} has changed" },
                        { VALUE_OLD, "Old value" },
                        { VALUE_NEW, "New value" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
