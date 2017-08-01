using Discord;

namespace TitanBot.Replying
{
    public interface IEmbedable
    {
        Embed GetEmbed();
        string GetString();
    }
}
