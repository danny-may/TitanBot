namespace TitanBotBase.Database.Tables
{
    public class Guild : IDbRecord
    {
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public ulong PermOverride { get; set; }
        public ulong[] RoleOverride { get; set; }
        public ulong[] BlackListed { get; set; }
    }
}
