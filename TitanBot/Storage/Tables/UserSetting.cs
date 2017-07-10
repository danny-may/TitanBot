using TitanBot.TextResource;

namespace TitanBot.Storage
{
    public class UserSetting : IDbRecord
    {
        public ulong Id { get; set; }
        public bool AltFormat { get; set; } = true;
        public Language Language { get; set; } = Language.EN;
    }
}
