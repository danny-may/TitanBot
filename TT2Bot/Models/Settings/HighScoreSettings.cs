using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT2Bot.Models.Settings
{
    class HighScoreSettings
    {
        public int DataStartRow { get; set; } = 4;
        public int RankingCol { get; set; } = 0;
        public int NameCol { get; set; } = 1;
        public int ClanCol { get; set; } = 2;
        public int RankMsCol { get; set; } = 3;
        public int SimMSCol { get; set; } = 4;
        public int RankRelCol { get; set; } = 5;
        public int TotalRelicsCol { get; set; } = 6;
        public int RawADCol { get; set; } = 7;
        public int FullADCol { get; set; } = 8;
    }
}
