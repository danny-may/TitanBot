using Discord;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Replying
{
    public interface IEmbedable
    {
        ILocalisable<EmbedBuilder> GetEmbed();
        ILocalisable<string> GetString();
    }
}
