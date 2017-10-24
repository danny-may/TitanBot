using Discord;

namespace TitanBot.Core.Models.Contexts
{
    public interface IUserContext : ITranslationContext
    {
        IUser User { get; }
    }
}