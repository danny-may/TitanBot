using LiteDB;

namespace TitanBot2.Database.Models
{
    public class Guild
    {
        [BsonId]
        public ulong GuildId { get; set; }
        public string Prefix { get; set; }
        public ulong? NotifyAlive { get; set; }
        public ulong? NotifyDead { get; set; }
        public ulong PermOverride { get; set; } = 8;
        public TitanLordSettings TitanLord { get; set; } = new TitanLordSettings();

        public class TitanLordSettings
        {
            public ulong? Channel { get; set; }
            public int CQ { get; set; } = 1;
            public int[] PrePings { get; set; } = new int[] { 300 };
            public string TimerText { get; set; } = ":alarm_clock: **Titan Lord Timer** :alarm_clock:\n```css\nThere is a Titan Lord ready in %TIME%```";
            public string InXText { get; set; } = "@everyone get your tapping fingers ready! There is a Titan Lord ready in %TIME% - %USER%";
            public string NowText { get; set; } = "@everyone there is a Titan Lord up right now! Lets make short work of it. - %USER%";
            public bool PinTimer { get; set; } = false;
        }
    }
}
