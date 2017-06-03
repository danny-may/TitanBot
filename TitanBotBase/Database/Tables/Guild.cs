namespace TitanBotBase.Database.Tables
{
    public class Guild : IBotDbRecord
    {
        public ulong GuildId { get; set; }
        public string TestText { get; set; }
    }
}
