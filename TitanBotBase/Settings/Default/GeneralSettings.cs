namespace TitanBotBase.Settings
{
    public class GuildSettings : ISettingGroup
    {
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public ulong PermOverride { get; set; } = 8;
        public ulong[] RoleOverride { get; set; }
        public ulong[] BlackListed { get; set; }
    }
}
