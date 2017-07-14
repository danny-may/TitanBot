using TitanBot.Formatting;

namespace TitanBot.Storage
{
    public class UserSetting : IDbRecord
    {
        public ulong Id { get; set; }
        public bool AltFormat { get; set; } = true;
        public Locale Language { get; set; } = Locale.DEFAULT;
    }
}
