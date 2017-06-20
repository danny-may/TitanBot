namespace TitanBot.Settings
{
    public class GeneralSettings
    {
        public string Prefix { get; set; }
        public ulong PermOverride { get; set; } = 8;
        public ulong[] RoleOverride { get; set; } = new ulong[0];
        public ulong[] BlackListed { get; set; } = new ulong[0];
    }
}
