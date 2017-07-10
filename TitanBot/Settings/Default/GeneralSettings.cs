using TitanBot.TextResource;

namespace TitanBot.Settings
{
    public class GeneralSettings
    {
        public string Prefix { get; set; }
        public ulong PermOverride { get; set; } = 8;
        public ulong[] RoleOverride { get; set; } = new ulong[0];
        public ulong[] BlackListed { get; set; } = new ulong[0];
        public string DateTimeFormat { get; set; } = "hh:mm:ss";
        public Language PreferredLanguage { get; set; } = Language.EN;
    }
}
