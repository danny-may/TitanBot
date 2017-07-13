using Discord;
using Discord.WebSocket;
using TitanBot.Dependencies;
using TitanBot.Settings;
using TitanBot.TextResource;

namespace TitanBot.Commands
{
    public interface ICommandContext
    {
        IUser Author { get; }
        IDependencyFactory DependencyFactory { get; }
        IMessageChannel Channel { get; }
        DiscordSocketClient Client { get; }
        IGuild Guild { get; }
        GeneralSettings GuildData { get; }
        IUserMessage Message { get; }
        int ArgPos { get; }
        string Prefix { get; }
        string CommandText { get; }
        CommandInfo? Command { get; }
        bool IsCommand { get; }
        bool ExplicitPrefix { get; }

        void CheckCommand(ICommandService commandService, string defaultPrefix);
        string[] SplitArguments(bool ignoreFlags, out (string Key, string Value)[] flags, int? maxLength = null, int? densePos = null);
    }
}