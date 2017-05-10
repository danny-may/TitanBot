using Discord.Commands;

namespace TitanBot2.Extensions
{
    public static class CommandServiceExtensions
    {
        public static SearchResult SearchModules(this CommandService service, ICommandContext context, string input)
        {
            return SearchResult.FromError(CommandError.UnknownCommand, "Command/Module not found");
        }
    }
}
