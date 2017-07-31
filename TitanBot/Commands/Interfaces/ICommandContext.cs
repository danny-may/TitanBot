namespace TitanBot.Commands
{
    public interface ICommandContext : IMessageContext
    {
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