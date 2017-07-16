using Discord;

namespace TitanBot.Commands
{
    public interface IEmbedable
    {
        Embed GetEmbed();
        string GetString();
    }
}
