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
        public static class Logic
        {
            private const string BASE_PATH = "LOGIC_";

            public const string COMMANDEXECUTOR_COMMAND_UNKNOWN = BASE_PATH + nameof(COMMANDEXECUTOR_COMMAND_UNKNOWN);
            public const string COMMANDEXECUTOR_DISALLOWED_CHANNEL = BASE_PATH + nameof(COMMANDEXECUTOR_DISALLOWED_CHANNEL);
            public const string COMMANDEXECUTOR_SUBCALL_UNKNOWN = BASE_PATH + nameof(COMMANDEXECUTOR_SUBCALL_UNKNOWN);
            public const string COMMANDEXECUTOR_SUBCALL_UNSPECIFIED = BASE_PATH + nameof(COMMANDEXECUTOR_SUBCALL_UNSPECIFIED);
            public const string COMMANDEXECUTOR_ARGUMENTS_TOOMANY = BASE_PATH + nameof(COMMANDEXECUTOR_ARGUMENTS_TOOMANY);
            public const string COMMANDEXECUTOR_ARGUMENTS_TOOFEW = BASE_PATH + nameof(COMMANDEXECUTOR_ARGUMENTS_TOOFEW);
            public const string COMMANDEXECUTOR_EXCEPTION_ALERT = BASE_PATH + nameof(COMMANDEXECUTOR_EXCEPTION_ALERT);
            public const string PERMISSIONMANAGER_DISALLOWED_NOTHERE = BASE_PATH + nameof(PERMISSIONMANAGER_DISALLOWED_NOTHERE);
            public const string PERMISSIONMANAGER_DISALLOWED_NOTOWNER = BASE_PATH + nameof(PERMISSIONMANAGER_DISALLOWED_NOTOWNER);
            public const string PERMISSIONMANAGER_DISALLOWED_NOPERMISSION = BASE_PATH + nameof(PERMISSIONMANAGER_DISALLOWED_NOPERMISSION);
            public const string COMMAND_DELAY_DEFAULT = BASE_PATH + nameof(COMMAND_DELAY_DEFAULT);
            public const string TYPEREADER_UNABLETOREAD = BASE_PATH + nameof(TYPEREADER_UNABLETOREAD);
            public const string TYPEREADER_NOTYPEREADER = BASE_PATH + nameof(TYPEREADER_NOTYPEREADER);
            public const string TYPEREADER_ENTITY_NOTFOUND = BASE_PATH + nameof(TYPEREADER_ENTITY_NOTFOUND);
            public const string UNABLE_SEND = BASE_PATH + nameof(UNABLE_SEND);
            public const string MESSAGE_TOO_LONG = BASE_PATH + nameof(MESSAGE_TOO_LONG);
            public const string MESSAGE_CONTAINED_ATTACHMENT = BASE_PATH + nameof(MESSAGE_CONTAINED_ATTACHMENT);


            public static IReadOnlyDictionary<string, string> Defaults { get; }
                = new Dictionary<string, string>
                {
                    { COMMANDEXECUTOR_COMMAND_UNKNOWN, "`{0}{1}` is not a recognised command! Try using `{0}help` for a complete command list." },
                    { COMMANDEXECUTOR_DISALLOWED_CHANNEL, "You cannot use that command here!" },
                    { COMMANDEXECUTOR_SUBCALL_UNKNOWN, "That is not a recognised subcommand for `{0}{1}`! Try using `{0}help {1}` for usage info." },
                    { COMMANDEXECUTOR_SUBCALL_UNSPECIFIED, "You have not specified which sub call you would like to use! Try using `{0}help {1}` for usage info." },
                    { COMMANDEXECUTOR_ARGUMENTS_TOOMANY, "Too many arguments were supplied. Try using `{0}help {1}` for usage info." },
                    { COMMANDEXECUTOR_ARGUMENTS_TOOFEW, "Not enough arguments were supplied. Try using `{0}help {1}` for usage info." },
                    { COMMANDEXECUTOR_EXCEPTION_ALERT, "There was an error when processing your command! ({0}) Please contact Titansmasher asap and give him EXCEPTIONID: {1}" },
                    { PERMISSIONMANAGER_DISALLOWED_NOTHERE, "You cannot use that command here!" },
                    { PERMISSIONMANAGER_DISALLOWED_NOTOWNER, "Only owners can use that command!" },
                    { PERMISSIONMANAGER_DISALLOWED_NOPERMISSION, "You do not have permission to use that command!" },
                    { COMMAND_DELAY_DEFAULT, "This seems to be taking longer than expected..." },
                    { TYPEREADER_UNABLETOREAD, "I could not read `{2}` as {3}" },
                    { TYPEREADER_NOTYPEREADER, "No reader found for `{3}`" },
                    { TYPEREADER_ENTITY_NOTFOUND, "{3} `{2}` could not be found" },
                    { UNABLE_SEND, "I was unable to send this message in {0} because I did not have permission:\n{1}" },
                    { MESSAGE_TOO_LONG, "I tried to send a message that was too long! Here is that message." },
                    { MESSAGE_CONTAINED_ATTACHMENT, "This message also contained an attachment, however I was unable to include that here." }
                }.ToImmutableDictionary();
        }
    }
}
