using Discord;
using Discord.WebSocket;
using TitanBot.Formatting;
using TitanBot.Settings;

namespace TitanBot.Commands
{
    public interface ICommandContext
    {
        IUser Author { get; }
        IReplier Replier { get; }
        ITextResourceCollection TextResource { get; }
        ITextResourceManager TextManager { get; }
        ValueFormatter Formatter { get; }
        IMessageChannel Channel { get; }
        DiscordSocketClient Client { get; }
        IGuild Guild { get; }
        IUserMessage Message { get; }
        int ArgPos { get; }
        string Prefix { get; }
        string CommandText { get; }
        CommandInfo? Command { get; }
        bool IsCommand { get; }
        bool ExplicitPrefix { get; }

        GeneralGlobalSetting GeneralGlobalSetting { get; }
        GeneralGuildSetting GeneralGuildSetting { get; }
        GeneralUserSetting GeneralUserSetting { get; }

        void CheckCommand(ICommandService commandService, string defaultPrefix);
        string[] SplitArguments(bool ignoreFlags, out (string Key, string Value)[] flags, int? maxLength = null, int? densePos = null);
    }
}