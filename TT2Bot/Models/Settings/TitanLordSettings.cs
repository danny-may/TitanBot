using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT2Bot.Models
{
    class TitanLordSettings
    {
        public ulong? Channel { get; set; }
        public int CQ { get; set; } = 1;
        public int[] PrePings { get; set; } = new int[] { 300 };
        public string TimerText { get; set; } = ":alarm_clock: **Titan Lord Timer** :alarm_clock:\n```css\nThere is a Titan Lord ready in %TIME%```";
        public string InXText { get; set; } = "@everyone get your tapping fingers ready! There is a Titan Lord ready in %TIME% - %USER%";
        public string NowText { get; set; } = "@everyone there is a Titan Lord up right now! Lets make short work of it. - %USER%";
        public string RoundText { get; set; } = "@everyone ding ding! Its time for round %ROUND%";
        public bool PinTimer { get; set; } = false;
        public bool RoundPings { get; set; } = true;
    }
}
