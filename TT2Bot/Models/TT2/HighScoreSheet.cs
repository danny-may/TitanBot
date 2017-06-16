using Csv;
using System.Collections.Generic;
using System.Linq;
using TitanBotBase.Settings;
using TT2Bot.Models.Settings;

namespace TT2Bot.Models
{
    public class HighScoreSheet
    {
        public List<HighScoreUser> Users = new List<HighScoreUser>();

        internal HighScoreSheet(IEnumerable<ICsvLine> data, HighScoreSettings settings)
        {
            foreach (var user in data.Skip(settings.DataStartRow))
            {
                var parsed = HighScoreUser.FromCsv(user, settings);
                if (parsed != null && parsed.Valid)
                    Users.Add(parsed);
            }
        }

        public class HighScoreUser
        {
            public int Ranking { get; }
            public string UserName { get; }
            public string ClanName { get; }
            public string RankMs { get; }
            public string SimMS { get; }
            public string RankRel { get; }
            public string TotalRelics { get; }
            public string RawAD { get; }
            public string FullAD { get; }
            public bool Valid => !string.IsNullOrWhiteSpace(UserName) &&
                                 !string.IsNullOrWhiteSpace(ClanName) &&
                                 !string.IsNullOrWhiteSpace(RankMs) &&
                                 !string.IsNullOrWhiteSpace(SimMS) &&
                                 !string.IsNullOrWhiteSpace(RankRel) &&
                                 !string.IsNullOrWhiteSpace(TotalRelics) &&
                                 !string.IsNullOrWhiteSpace(RawAD) &&
                                 !string.IsNullOrWhiteSpace(FullAD);

            public HighScoreUser(int ranking,
                                 string userName,
                                 string clanName,
                                 string rankMs,
                                 string simMs,
                                 string rankRel,
                                 string totalRelics,
                                 string rawAd,
                                 string fullAd)
            {
                Ranking = ranking;
                UserName = userName;
                ClanName = clanName;
                RankMs = rankMs;
                SimMS = simMs;
                RankRel = rankRel;
                TotalRelics = totalRelics;
                RawAD = rawAd;
                FullAD = fullAd;
            }

            internal static HighScoreUser FromCsv(ICsvLine data, HighScoreSettings settings)
            {
                int ranking;
                string userName;
                string clanName;
                string rankMs;
                string simMs;
                string rankRel;
                string totalRelics;
                string rawAd;
                string fullAd;

                if (!int.TryParse(data[settings.RankingCol].Trim(), out ranking))
                    return null;
                userName = data[settings.NameCol].Trim();
                clanName = data[settings.ClanCol].Trim();
                rankMs = data[settings.RankMsCol].Trim();
                simMs = data[settings.SimMSCol].Trim();
                rankRel = data[settings.RankRelCol].Trim();
                totalRelics = data[settings.TotalRelicsCol].Trim();
                rawAd = data[settings.RawADCol].Trim();
                fullAd = data[settings.FullADCol].Trim();

                return new HighScoreUser(ranking, userName, clanName, rankMs, simMs, rankRel, totalRelics, rawAd, fullAd);
            }

            public override string ToString()
            {
                return $"#{Ranking} - {UserName} [{ClanName}]";
            }
        }
    }
}
