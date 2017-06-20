namespace TitanBot.Database
{
    public class UserSetting : IDbRecord
    {
        public ulong Id { get; set; }
        public bool AltFormat { get; set; } = true;
    }
}
