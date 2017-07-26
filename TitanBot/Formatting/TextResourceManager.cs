using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TitanBot.Formatting
{
    public class TextResourceManager : ITextResourceManager
    {
        private Dictionary<string, Dictionary<Locale, string>> TextMap { get; set; }
        static readonly string FileName = Path.Combine(AppContext.BaseDirectory, "Localisation.json");
        static readonly string DirectoryPath = Path.GetDirectoryName(FileName);

        private Dictionary<string, Dictionary<Locale, string>> Defaults { get; } = new Dictionary<string, Dictionary<Locale, string>>();

        public Locale[] SupportedLanguages => TextMap.SelectMany(t => t.Value.Keys).Distinct().ToArray();

        public TextResourceManager()
        {
            RequireKeys(GetDefaults());
            Refresh();
        }

        private string SanitiseKey(string key)
            => key.ToUpper().Replace(' ', '_');

        public void AddResource(string key, string text)
            => AddResource(key, Locale.DEFAULT, text);
        public void AddResource(string key, Locale language, string text)
        {
            key = SanitiseKey(key);
            if (!TextMap.ContainsKey(key))
                TextMap.Add(key, new Dictionary<Locale, string>());
            TextMap[key][language] = text;
        }

        public void SaveChanges()
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
            EnsureKeys();
            File.WriteAllText(FileName, JsonConvert.SerializeObject(TextMap.OrderBy(k => k.Key).ToDictionary(k => k.Key, k => k.Value), Newtonsoft.Json.Formatting.Indented));
        }

        public void Refresh()
        {
            if (!File.Exists(FileName))
                SaveChanges();
            TextMap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Locale, string>>>(File.ReadAllText(FileName));
            TextMap = TextMap.ToDictionary(v => SanitiseKey(v.Key), v => v.Value);
            SaveChanges();
        }

        public ITextResourceCollection GetForLanguage(Locale language, ValueFormatter valueFormatter)
        {
            return new TextResourceCollection( GetLanguageCoverage(language), valueFormatter,
                TextMap.SelectMany(k => k.Value.Select(v => (key: k.Key, language: v.Key, text: v.Value)))
                       .Where(v => v.language == language || v.language == Locale.DEFAULT)
                       .GroupBy(v => v.key)
                       .ToDictionary(v => v.Key, v => (v.FirstOrDefault(l => l.language == Locale.DEFAULT).text, v.FirstOrDefault(l => l.language == language).text))
                );
        }

        public double GetLanguageCoverage(Locale language)
        {
            var totalString = (double)TextMap.Count;
            var covered = (double)TextMap.Count(v => v.Value.ContainsKey(language));
            return covered / totalString;
        }

        private void EnsureKeys()
        {
            TextMap = TextMap ?? new Dictionary<string, Dictionary<Locale, string>>();
            var missing = Defaults.Where(d => !TextMap.ContainsKey(d.Key));
            foreach (var key in missing)
                TextMap[key.Key] = new Dictionary<Locale, string>();

            foreach (var key in Defaults)
                foreach (var vals in key.Value)
                    if (!TextMap[key.Key].ContainsKey(vals.Key))
                        TextMap[key.Key][vals.Key] = vals.Value;
        }

        public void RequireKeys(Dictionary<string, string> values)
        {
            values = values ?? new Dictionary<string, string>();
            foreach (var pair in values)
            {
                var key = SanitiseKey(pair.Key);
                if (!Defaults.ContainsKey(key))
                    Defaults[key] = new Dictionary<Locale, string>();
                Defaults[key][Locale.DEFAULT] = pair.Value;
            }

            EnsureKeys();
        }

        private static Dictionary<string, string> GetDefaults()
            =>  new Dictionary<string, string>
            {
                //Help Stuff

                {"PING_HELP_DESCRIPTION", "Basic command for calculating the delay of the bot."},
                {"PING_HELP_USAGE", "Replies with a pong and what the current delay is."},
                {"EDITCOMMAND_HELP_DESCRIPTION", "Used to allow people with varying roles or permissions to use different commands."},
                {"EDITCOMMAND_HELP_NOTES", "To work out just what permission id you need, give the [permission calculator](https://discordapi.com/permissions.html) a try!"},
                {"EDITCOMMAND_HELP_USAGE_SETROLE", "Sets a list of roles required to use each command supplied"},
                {"EDITCOMMAND_HELP_USAGE_SETPERM", "Sets a permission required to use each command supplied"},
                {"EDITCOMMAND_HELP_USAGE_RESET", "Resets the roles and permissions required to use each command supplied"},
                {"EDITCOMMAND_HELP_USAGE_BLACKLIST", "Prevents anyone with permissions below the override permissions from using the command in the given channel"},
                {"SETTINGS_HELP_DESCRIPTION", "Allows the retrieval and changing of existing settings for the server"},
                {"SETTINGS_HELP_USAGE_DEFAULT", "Lists all settings available"},
                {"SETTINGS_HELP_USAGE_SET", "Sets the given setting to the given value."},
                {"SETTINGSRESET_HELP_DESCRIPTION", "Resets all settings and command permissions for a guild."},
                {"SETTINGSRESET_HELP_USAGE_THISGUILD", "Resets settings for this guild"},
                {"SETTINGSRESET_HELP_USAGE_GIVENGUILD", "Resets the given guild"},
                {"ABOUT_HELP_DESCRIPTION", "A tiny command that just displays some helpful links :)"},
                {"ABOUT_HELP_USAGE", "Shows some about text for me"},
                {"DONATE_HELP_DESCRIPTION", "You.. youre thinking of donating? :open_mouth: This command will give you a link to my patreon page"},
                {"DONATE_HELP_USAGE", "Gives you the link you awesome person :heart:"},
                {"HELP_HELP_DESCRIPTION", "Displays help for any command"},
                {"HELP_HELP_USAGE", "Displays a list of all commands, or help for a single command"},
                {"INFO_HELP_DESCRIPTION", "Displays some technical information about me"},
                {"INFO_HELP_USAGE_LANGUAGE", "Displays info about what languages are supported"},
                {"INFO_HELP_USAGE_TECHNICAL", "Displays technical info for me"},
                {"INVITE_HELP_DESCRIPTION", "Provides a link to invite me to any guild"},
                {"INVITE_HELP_USAGE", "Shows the invite link"},
                {"PREFIX_HELP_DESCRIPTION", "Gets or sets a custom prefix that is required to use my commands"},
                {"PREFIX_HELP_USAGE_SHOW", "Gets all the available current prefixes"},
                {"PREFIX_HELP_USAGE_SET", "Sets the custom prefix"},
                {"DBPURGE_HELP_DESCRIPTION", "Wipes any database table clean"},
                {"DBPURGE_HELP_USAGE", "Wipes the given table."},
                {"EXEC_HELP_DESCRIPTION", "Allows for arbitrary code execution"},
                {"EXEC_HELP_NOTES", "https://github.com/Titansmasher/TitanBot/blob/rewrite/TitanBotBase/Commands/DefaultCommands/Owner/ExecCommand.cs#L111"},
                {"EXEC_HELP_USAGE", "Executes arbitrary code"},
                {"GLOBALSETTINGS_HELP_DESCRIPTION", "Allows the retrieval and changing of existing settings for the server"},
                {"GLOBALSETTINGS_HELP_USAGE_DEFAULT", "Lists all settings available"},
                {"GLOBALSETTINGS_HELP_USAGE_SET", "Sets the given setting to the given value."},
                {"SHUTDOWN_HELP_DESCRIPTION", "Shuts me down"},
                {"SHUTDOWN_HELP_USAGE", "Shuts me down now, or in the time provided"},
                {"SUDOCOMMAND_HELP_DESCRIPTION", "Executes any command on behalf of another user. Requires a prefix in the message"},
                {"SUDOCOMMAND_HELP_USAGE", "Will execute any command as the given user"},
                {"PREFERENCES_HELP_DESCRIPTION", "Allows you to set some options for how the bot will interact with you."},
                {"PREFERENCES_HELP_USAGE_DEFAULT", "Lists all preferences that are available"},
                {"PREFERENCES_HELP_USAGE_SET", "Sets the given preference to the given value"},
                {"RELOAD_HELP_DESCRIPTION", "Reloads a given area of the bot"},
                {"RELOAD_HELP_USAGE", "Reloads the supplied area" },
                {"RELOAD_HELP_USAGE_LIST", "Lists all available areas to reload" },

                //Settings stuff

                {"SETTINGS_GUILD_GENERAL_DESCRIPTION", "General guild settings"},
                {"SETTINGS_GUILD_GENERAL_NOTES", "For DateTimeFormat, you can use [this link](https://www.codeproject.com/Articles/19677/Formats-for-DateTime-ToString) to help determine what is and is not valid!"},
                {"SETTINGS_GLOBAL_GENERAL_DESCRIPTION", "General global settings"},
                {"SETTINGS_USER_GENERAL_DESCRIPTION", "General user settings"},
                {"LOCALE_UNKNOWN", "The locale `{0}` does not yet exist."},
                {"FORMATTINGTYPE_UNKNOWN", "The formatting type `{0}` does not exist."},

                //Bot stuff

                {"COMMANDEXECUTOR_COMMAND_UNKNOWN", "`{0}{1}` is not a recognised command! Try using `{0}help` for a complete command list."},
                {"COMMANDEXECUTOR_DISALLOWED_CHANNEL", "You cannot use that command here!"},
                {"COMMANDEXECUTOR_SUBCALL_UNKNOWN", "That is not a recognised subcommand for `{0}{1}`! Try using `{0}help {1}` for usage info."},
                {"COMMANDEXECUTOR_SUBCALL_UNSPECIFIED", "You have not specified which sub call you would like to use! Try using `{0}help {1}` for usage info."},
                {"COMMANDEXECUTOR_ARGUMENTS_TOOMANY", "Too many arguments were supplied. Try using `{0}help {1}` for usage info."},
                {"COMMANDEXECUTOR_ARGUMENTS_TOOFEW", "Not enough arguments were supplied. Try using `{0}help {1}` for usage info."},
                {"COMMANDEXECUTOR_EXCEPTION_ALERT", "There was an error when processing your command! ({0}) Please contact Titansmasher asap and give him EXCEPTIONID: {1}"},
                {"PERMISSIONMANAGER_DISALLOWED_NOTHERE", "You cannot use that command here!"},
                {"PERMISSIONMANAGER_DISALLOWED_NOTOWNER", "Only owners can use that command!"},
                {"PERMISSIONMANAGER_DISALLOWED_NOPERMISSION", "You do not have permission to use that command!"},
                {"COMMAND_DELAY_DEFAULT", "This seems to be taking longer than expected..."},
                {"TYPEREADER_UNABLETOREAD", "I could not read `{2}` as {3}"},
                {"TYPEREADER_NOTYPEREADER", "No reader found for `{3}`"},
                {"TYPEREADER_ENTITY_NOTFOUND", "{3} `{2}` could not be found"},
                {"UNABLE_SEND", "I was unable to send this message in {0} because I did not have permission:\n{1}"},
                {"MESSAGE_TOO_LONG", "I tried to send a message that was too long! Here is that message."},
                {"MESSAGE_CONTAINED_ATTACHMENT", "This message also contained an attachment, however I was unable to include that here."},

                //Command stuff

                {"PING_INITIAL", "| ~{0} ms"},
                {"PING_VERIFY", "| {0} ms"},
                {"INFO_LANGUAGE_EMBED_DESCRIPTION", "Here are all the supported languages by me!"},
                {"EDITCOMMAND_FINDCALLS_NORESULTS", "There were no commands that matched those calls."},
                {"EDITCOMMAND_SUCCESS", "{0} set successfully for {1} command(s)!"},
                {"SETTINGS_FOOTERTEXT", "{0} | Settings"},
                {"SETTINGS_TITLE_NOGROUP", "Please select a setting group from the following:"},
                {"SETTINGS_DESCRIPTION_NOSETTINGS", "No settings groups available!"},
                {"SETTINGS_INVALIDGROUP", "`{0}`isnt a valid setting group!"},
                {"SETTINGS_TITLE_GROUP", "Here are all the settings for the group `{0}`"},
                {"SETTINGS_NOTSET", "Not Set"},
                {"SETTINGS_KEY_NOTFOUND", "Could not find the `{0}` setting"},
                {"SETTINGS_VALUE_INVALID", "`{1}` is not a valid value for the setting {0}"},
                {"SETTINGS_VALUE_CHANGED_TITLE", "{0} has changed"},
                {"SETTING_VALUE_OLD", "Old value"},
                {"SETTING_VALUE_NEW", "New value"},
                {"SETTINGRESET_GUILD_NOTEXIST", "That guild does not exist."},
                {"SETTINGRESET_SUCCESS", "All settings deleted for {0}({1})"},
                {"ABOUT_MESSAGE", "'Ello there :wave:, Im Titanbot! Im an open source discord bot library written in C# by Titansmasher, with a fair bit of help from Mildan too!.\n" +
                                             "This is unfortunately just my base functionality, however there are other bots that run using this bot as its core library.\n" +
                                             "If you wish to use this library to make your own bot, feel free to do so! You can find me on github here: <https://github.com/Titansmasher/TitanBot>\n\n" +
                                             "_If you are seeing this on a fully fledged bot, please contact the owner of said bot because they appear to have not supplied their own about message_"},
                {"DONATE_MESSAGE_ADDITIONAL", ""},
                {"HELP_LIST_TITLE", "These are the commands you can use"},
                {"HELP_LIST_DESCRIPTION", "To use a command, type `<prefix><command>`\n  e.g. `{0}help`\n" +
                                                     "To pass arguments, add a list of values after the command separated by space\n  e.g. `{0}command a, b, c, d`\n" +
                                                     "You can use any of these prefixes: \"{1}\""},
                {"HELP_SINGLE_UNRECOGNISED", "`{0}` is not a recognised command. Use `{1}help` for a list of all available commands"},
                {"HELP_SINGLE_NOUSAGE", "No usage available!"},
                {"HELP_SINGLE_NODESCRIPTION", "No description!"},
                {"HELP_SINGLE_NOGROUP", "No categories!"},
                {"HELP_SINGLE_TITLE", "Help for {0}"},
                {"HELP_SINGLE_USAGE_FOOTER", "\n\n_`<param>` = required\n`[param]` = optional\n`<param...>` = accepts multiple (comma separated)_"},
                {"INFO_FIELD_GUILDS", "Guilds"},
                {"INFO_FIELD_CHANNELS", "Channels"},
                {"INFO_FIELD_USERS", "Users"},
                {"INFO_FIELD_COMMANDS", "Loaded commands"},
                {"INFO_FIELD_CALLS", "Loaded calls"},
                {"INFO_FIELD_COMMANDS_USED", "Commands used this session"},
                {"INFO_FIELD_DATABASE_QUERIES", "Database queries"},
                {"INFO_FIELD_RAM", "RAM"},
                {"INFO_FIELD_CPU", "CPU"},
                {"INFO_FIELD_TIMERS", "Active Timers"},
                {"INFO_FIELD_UPTIME", "Uptime"},
                {"INFO_LANGUAGE_COVERAGE", "{0}% Coverage"},
                {"INFO_TECHNICAL_TITLE", "Current execution statistics"},
                {"INVITE_MESSAGE", "Want to invite me to your guild? Click this link!\n<https://discordapp.com/oauth2/authorize?client_id={0}&scope=bot&permissions={1}>"},
                {"PREFIX_SHOW_NOPREFIX", "You do not require prefixes in this channel"},
                {"PREFIX_SHOW_MESSAGE", "Your available prefixes are {0)}"},
                {"PREFIX_SET_MESSAGE", "Your guilds prefix has been set to `{0}`"},
                {"DBPURGE_SUCCESS", "Attempted to drop all data from the `{0}` table"},
                {"EXEC_FOOTER_CONSTRUCTFAILED", "Failed to construct. Took {0}ms"},
                {"EXEC_FOOTER_COMPILEFAILED", "Constructed in: {0}ms | Failed to compile. Took {1}ms"},
                {"EXEC_FOOTER_EXECUTEFAILED", "Constructed in: {0}ms | Compiled in: {1}ms | Failed to execute. Took {2}ms"},
                {"EXEC_FOOTER_SUCCESS", "Constructed in: {0}ms | Compiled in: {1}ms | Executed in: {2}ms"},
                {"EXEC_INPUT_FORMAT", "```csharp\n{0}\n```"},
                {"EXEC_OUTPUT_NULL", "No output from execution..."},
                {"EXEC_OUTPUT_FORMAT", "Type: {0}\n```csharp\n{1)}\n```"},
                {"EXEC_TITLE_EXCEPTION", ":no_entry_sign: Execution Result"},
                {"EXEC_TITLE_SUCCESS", ":white_check_mark: Execution Result"},
                {"SHUTDOWN_INTIME", "Shutting down in {0}"},
                {"SUDOCOMMAND_SUCCESS", "Executing `{0}` as {1}"},
                {"EXCEPTION_NOTFOUND", "Exception {0} has not yet occured!"},
                {"EXCEPTION_MESSAGE", "Message"},
                {"EXCEPTION_CHANNEL", "Channel"},
                {"EXCEPTION_GUILD", "Guild"},
                {"EXCEPTION_FULLMESSAGE", "Here is everything that was logged for exception {0}"},
                {"RELOAD_AREA_NOTFOUND", "The area `{0}` does not exist" },
                {"RELOAD_SUCCESS", "Reloading {0}"},
                {"RELOAD_LIST", "Here are all the areas that can be reloaded:\n{0}" },

                //General stuff

                {"EMBED_FOOTER", "{0} | {1}"},
                {"REPLYTYPE_SUCCESS", ":white_check_mark: **Got it!** "},
                {"REPLYTYPE_ERROR", ":no_entry_sign: **Oops!** "},
                {"REPLYTYPE_INFO", ":information_source: "},
                {"NOTES", "Notes"},
                {"GROUP", "Group"},
                {"ALIASES", "Aliases"},
                {"USAGE", "Usage"},
                {"FLAGS", "Flags"},
                {"INPUT", "Input"},
                {"ERROR", "Error"},
                {"OUTPUT", "Output"},

                //Types

                {"TIMESPAN", "a timespan"},
                {"STRING", "some text"},
                {"INT64", "an integer"},
                {"INT32", "an integer"},
                {"INT16", "an integer"},
                {"UINT64", "a positive integer"},
                {"UINT32", "a positive integer"},
                {"UINT16", "a positive integer"},
                {"DOUBLE", "a number"},
                {"FLOAT", "a number"},
                {"DECIMAL", "a number"},
                {"BOOLEAN", "true/false"},
                {"DATETIME", "a date/time"},
            };
    }
}
