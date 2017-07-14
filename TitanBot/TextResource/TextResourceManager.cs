using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.TextResource
{
    public class TextResourceManager : ITextResourceManager
    {
        private Dictionary<string, Dictionary<Locale, string>> TextMap { get; set; }

        public Locale[] SupportedLanguages => TextMap.SelectMany(t => t.Value.Keys).Distinct().ToArray();

        public void AddResource(string key, string text)
            => AddResource(key, Locale.DEFAULT, text);
        public void AddResource(string key, Locale language, string text)
        {
            if (!TextMap.ContainsKey(key))
                TextMap.Add(key, new Dictionary<Locale, string>());
            TextMap[key][language] = text;
        }

        string FileName = "res\\text.json";

        public TextResourceManager()
        {
            Refresh();
        }

        public void Refresh()
        {

            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file))
            {
                string path = Path.GetDirectoryName(file);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                LoadDefaults();
                File.WriteAllText(file, JsonConvert.SerializeObject(TextMap, Formatting.Indented));
            }
            TextMap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Locale, string>>>(File.ReadAllText(file));
        }

        public ITextResourceCollection GetForLanguage(Locale language)
        {
            return new TextResourceCollection( GetLanguageCoverage(language),
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

        private void LoadDefaults()
        {
            TextMap = new Dictionary<string, Dictionary<Locale, string>>();

            //Help Stuff

            AddResource("PING_HELP_DESCRIPTION", "Basic command for calculating the delay of the bot.");
            AddResource("PING_HELP_USAGE", "Replies with a pong and what the current delay is.");
            AddResource("EDITCOMMAND_HELP_DESCRIPTION", "Used to allow people with varying roles or permissions to use different commands.");
            AddResource("EDITCOMMAND_HELP_NOTES", "To work out just what permission id you need, give the [permission calculator](https://discordapi.com/permissions.html) a try!");
            AddResource("EDITCOMMAND_HELP_USAGE_SETROLE", "Sets a list of roles required to use each command supplied");
            AddResource("EDITCOMMAND_HELP_USAGE_SETPERM", "Sets a permission required to use each command supplied");
            AddResource("EDITCOMMAND_HELP_USAGE_RESET", "Resets the roles and permissions required to use each command supplied");
            AddResource("EDITCOMMAND_HELP_USAGE_BLACKLIST", "Prevents anyone with permissions below the override permissions from using the command in the given channel");
            AddResource("SETTINGS_HELP_DESCRIPTION", "Allows the retrieval and changing of existing settings for the server");
            AddResource("SETTINGS_HELP_USAGE_DEFAULT", "Lists all settings available");
            AddResource("SETTINGS_HELP_USAGE_SET", "Sets the given setting to the given value.");
            AddResource("SETTINGSRESET_HELP_DESCRIPTION", "Resets all settings and command permissions for a guild.");
            AddResource("SETTINGSRESET_HELP_USAGE_THISGUILD", "Resets settings for this guild");
            AddResource("SETTINGSRESET_HELP_USAGE_GIVENGUILD", "Resets the given guild");
            AddResource("ABOUT_HELP_DESCRIPTION", "A tiny command that just displays some helpful links :)");
            AddResource("ABOUT_HELP_USAGE", "Shows some about text for me");
            AddResource("DONATE_HELP_DESCRIPTION", "You.. youre thinking of donating? :open_mouth: This command will give you a link to my patreon page");
            AddResource("DONATE_HELP_USAGE", "Gives you the link you awesome person :heart:");
            AddResource("HELP_HELP_DESCRIPTION", "Displays help for any command");
            AddResource("HELP_HELP_USAGE", "Displays a list of all commands, or help for a single command");
            AddResource("INFO_HELP_DESCRIPTION", "Displays some technical information about me");
            AddResource("INFO_HELP_USAGE_LANGUAGE", "Displays info about what languages are supported");
            AddResource("INFO_HELP_USAGE_TECHNICAL", "Displays technical info for me");
            AddResource("INVITE_HELP_DESCRIPTION", "Provides a link to invite me to any guild");
            AddResource("INVITE_HELP_USAGE", "Shows the invite link");
            AddResource("PREFIX_HELP_DESCRIPTION", "Gets or sets a custom prefix that is required to use my commands");
            AddResource("PREFIX_HELP_USAGE_SHOW", "Gets all the available current prefixes");
            AddResource("PREFIX_HELP_USAGE_SET", "Sets the custom prefix");
            AddResource("DBPURGE_HELP_DESCRIPTION", "Wipes any database table clean");
            AddResource("DBPURGE_HELP_USAGE", "Wipes the given table.");
            AddResource("EXEC_HELP_DESCRIPTION", "Allows for arbitrary code execution");
            AddResource("EXEC_HELP_NOTES", "https://github.com/Titansmasher/TitanBot/blob/rewrite/TitanBotBase/Commands/DefaultCommands/Owner/ExecCommand.cs#L111");
            AddResource("EXEC_HELP_USAGE", "Executes arbitrary code");
            AddResource("GLOBALSETTINGS_HELP_DESCRIPTION", "Allows the retrieval and changing of existing settings for the server");
            AddResource("GLOBALSETTINGS_HELP_USAGE_DEFAULT", "Lists all settings available");
            AddResource("GLOBALSETTINGS_HELP_USAGE_SET", "Sets the given setting to the given value.");
            AddResource("SHUTDOWN_HELP_DESCRIPTION", "Shuts me down");
            AddResource("SHUTDOWN_HELP_USAGE", "Shuts me down now, or in the time provided");
            AddResource("SUDOCOMMAND_HELP_DESCRIPTION", "Executes any command on behalf of another user. Requires a prefix in the message");
            AddResource("SUDOCOMMAND_HELP_USAGE", "Will execute any command as the given user");

            //Settings stuff

            AddResource("SETTINGS_GENERAL_DESCRIPTION", "General settings for the bot");
            AddResource("SETTINGS_GENERAL_NOTES", "For DateTimeFormat, you can use [this link](https://www.codeproject.com/Articles/19677/Formats-for-DateTime-ToString) to help determine what is and is not valid!");
            AddResource("SETTINGS_GLOBAL_GENERAL_DESCRIPTION", "General global settings");

            //Bot stuff

            AddResource("COMMANDEXECUTOR_COMMAND_UNKNOWN", "{0}{1} is not a recognised command! Try using `{0}help` for a complete command list.");
            AddResource("COMMANDEXECUTOR_DISALLOWED_CHANNEL", "You cannot use that command here!");
            AddResource("COMMANDEXECUTOR_SUBCALL_UNKNOW", "That is not a recognised subcommand for `{0}{1}`! Try using `{0}help {1}` for usage info.");
            AddResource("COMMANDEXECUTOR_SUBCALL_UNSPECIFIED", "You have not specified which sub call you would like to use! Try using `{0}help {1}` for usage info.");

            //Command stuff

            AddResource("PING_INITIAL", "| ~{0} ms");
            AddResource("PING_VERIFY", "| {0} ms");
            AddResource("INFO_LANGUAGE_EMBED_DESCRIPTION", "Here are all the supported languages by me!");
            AddResource("EDITCOMMAND_FINDCALLS_NORESULTS", "There were no commands that matched those calls.");
            AddResource("EDITCOMMAND_SUCCESS", "{0} set successfully for {1} command(s)!");
            AddResource("SETTINGS_FOOTERTEXT", "{0} | Settings");
            AddResource("SETTINGS_TITLE_NOGROUP", "Please select a setting group from the following:");
            AddResource("SETTINGS_DESCRIPTION_NOSETTINGS", "No settings groups available!");
            AddResource("SETTINGS_INVALIDGROUP", "`{0}`isnt a valid setting group!");
            AddResource("SETTINGS_TITLE_GROUP", "Here are all the settings for the group `{0}`");
            AddResource("SETTINGS_NOTSET", "Not Set");
            AddResource("SETTINGS_KEY_NOTFOUND", "Could not find the `{0}` setting");
            AddResource("SETTINGS_VALUE_INVALID", "`{1}` is not a valid value for the setting {0}");
            AddResource("SETTINGS_VALUE_CHANGED_TITLE", "{0} has changed");
            AddResource("SETTING_VALUE_OLD", "Old value");
            AddResource("SETTING_VALUE_NEW", "New value");
            AddResource("SETTINGRESET_GUILD_NOTEXIST", "That guild does not exist.");
            AddResource("SETTINGRESET_SUCCESS", "All settings deleted for {0}({1})");
            AddResource("ABOUT_MESSAGE", "'Ello there :wave:, Im Titanbot! Im an open source discord bot library written in C# by Titansmasher, with a fair bit of help from Mildan too!.\n" +
                                         "This is unfortunately just my base functionality, however there are other bots that run using this bot as its core library.\n" +
                                         "If you wish to use this library to make your own bot, feel free to do so! You can find me on github here: <https://github.com/Titansmasher/TitanBot>\n\n" +
                                         "_If you are seeing this on a fully fledged bot, please contact the owner of said bot because they appear to have not supplied their own about message_");
            AddResource("DONATE_MESSAGE_ADDITIONAL", "");
            AddResource("HELP_LIST_TITLE", "These are the commands you can use");
            AddResource("HELP_LIST_DESCRIPTION", "To use a command, type `<prefix><command>`\n  e.g. `{0}help`\n" +
                                                 "To pass arguments, add a list of values after the command separated by space\n  e.g. `{0}command a, b, c, d`\n" +
                                                 "You can use any of these prefixes: \"{1}\"");
            AddResource("HELP_SINGLE_UNRECOGNISED", "`{0}` is not a recognised command. Use `{1}help` for a list of all available commands");
            AddResource("HELP_SINGLE_NOUSAGE", "No usage available!");
            AddResource("HELP_SINGLE_NODESCRIPTION", "No description!");
            AddResource("HELP_SINGLE_NOGROUP", "No categories!");
            AddResource("HELP_SINGLE_TITLE", "Help for {0}");
            AddResource("HELP_SINGLE_USAGE_FOOTER", "\n\n_`<param>` = required\n`[param]` = optional\n`<param...>` = accepts multiple (comma separated)_");
            AddResource("INFO_FIELD_GUILDS", "Guilds");
            AddResource("INFO_FIELD_CHANNELS", "Channels");
            AddResource("INFO_FIELD_USERS", "Users");
            AddResource("INFO_FIELD_COMMANDS", "Loaded commands");
            AddResource("INFO_FIELD_CALLS", "Loaded calls");
            AddResource("INFO_FIELD_COMMANDS_USED", "Commands used this session");
            AddResource("INFO_FIELD_DATABASE_QUERIES", "Database queries");
            AddResource("INFO_FIELD_RAM", "RAM");
            AddResource("INFO_FIELD_CPU", "CPU");
            AddResource("INFO_FIELD_TIMERS", "Active Timers");
            AddResource("INFO_FIELD_UPTIME", "Uptime");
            AddResource("INFO_LANGUAGE_COVERAGE", "{0}% Coverage");
            AddResource("INFO_TECHNICAL_TITLE", "Current execution statistics");
            AddResource("INVITE_MESSAGE", "Want to invite me to your guild? Click this link!\n<https://discordapp.com/oauth2/authorize?client_id={0}&scope=bot&permissions={1}>");
            AddResource("PREFIX_SHOW_NOPREFIX", "You do not require prefixes in this channel");
            AddResource("PREFIX_SHOW_MESSAGE", "Your available prefixes are {0)}");
            AddResource("PREFIX_SET_MESSAGE", "Your guilds prefix has been set to `{0}`");
            AddResource("DBPURGE_SUCCESS", "Attempted to drop all data from the `{0}` table");
            AddResource("EXEC_FOOTER_CONSTRUCTFAILED", "Failed to construct. Took {0}ms");
            AddResource("EXEC_FOOTER_COMPILEFAILED", "Constructed in: {0}ms | Failed to compile. Took {1}ms");
            AddResource("EXEC_FOOTER_EXECUTEFAILED", "Constructed in: {0}ms | Compiled in: {1}ms | Failed to execute. Took {2}ms");
            AddResource("EXEC_FOOTER_SUCCESS", "Constructed in: {0}ms | Compiled in: {1}ms | Executed in: {2}ms");
            AddResource("EXEC_INPUT_FORMAT", "```csharp\n{0}\n```");
            AddResource("EXEC_OUTPUT_NULL", "No output from execution...");
            AddResource("EXEC_OUTPUT_FORMAT", "Type: {0}\n```csharp\n{1)}\n```");
            AddResource("EXEC_TITLE_EXCEPTION", ":no_entry_sign: Execution Result");
            AddResource("EXEC_TITLE_SUCCESS", ":white_check_mark: Execution Result");
            AddResource("SHUTDOWN_INTIME", "Shutting down in {0}");
            AddResource("SUDOCOMMAND_SUCCESS", "Executing `{0}` as {1}");

            //General stuff

            AddResource("EMBED_FOOTER", "{0} | {1}");
            AddResource("REPLYTYPE_SUCCESS", ":white_check_mark: **Got it!** ");
            AddResource("REPLYTYPE_ERROR", ":no_entry_sign: **Oops!** ");
            AddResource("REPLYTYPE_INFO", ":information_source: ");
            AddResource("NOTES", "Notes");
            AddResource("GROUP", "Group");
            AddResource("ALIASES", "Aliases");
            AddResource("USAGE", "Usage");
            AddResource("FLAGS", "Flags");
            AddResource("INPUT", "Input");
            AddResource("ERROR", "Error");
            AddResource("OUTPUT", "Output");

        }
    }
}
