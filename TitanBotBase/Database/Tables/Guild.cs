namespace TitanBotBase.Database.Tables
{
    public class Guild : IDbRecord
    {
        public ulong Id { get; set; }
        public string TestText { get; set; }
    }
}
