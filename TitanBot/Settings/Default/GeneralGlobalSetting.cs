namespace TitanBot.Settings
{
    public class GeneralGlobalSetting
    {
        public string Token { get; set; }
        public ulong[] Owners { get; set; } = new ulong[0];
        public string DefaultPrefix { get; set; } = "t$";
        public ulong PreferredPermission { get; set; } = 8;
    }
}
