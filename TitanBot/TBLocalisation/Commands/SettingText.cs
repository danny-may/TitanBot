using System.Collections.Generic;
using System.Collections.Immutable;

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
                public const string NO_GROUPS = BASE_PATH + nameof(NO_GROUPS);
                public const string GROUP_ID = BASE_PATH + nameof(GROUP_ID);
                public const string GROUP_SET = BASE_PATH + nameof(GROUP_SET);
                public const string GROUP_REMOVED = BASE_PATH + nameof(GROUP_REMOVED);
                public const string MISSING_GROUPID = BASE_PATH + nameof(MISSING_GROUPID);
                public const string MISSING_NAMES = BASE_PATH + nameof(MISSING_NAMES);
                public const string INVALID_METHOD = BASE_PATH + nameof(INVALID_METHOD);
                public const string GROUP_DISALLOWED = BASE_PATH + nameof(GROUP_DISALLOWED);

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
                        { VALUE_NEW, "New value" },
                        { NO_GROUPS, "You have not got any groups set up yet. Try using `{0}help {1}` for information on how to add groups" },
                        { GROUP_ID, "Group {0}" },
                        { GROUP_SET, "Set setting group {0} with the values `{1}`" },
                        { GROUP_REMOVED, "Removed setting group {0}" },
                        { MISSING_GROUPID, "You must supply a group Id for this method" },
                        { MISSING_NAMES, "You must supply atleast 1 name for this method" },
                        { INVALID_METHOD, "The method `{0}` was not recognised" },
                        { GROUP_DISALLOWED, "Groups are not enabled for this setting!" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
