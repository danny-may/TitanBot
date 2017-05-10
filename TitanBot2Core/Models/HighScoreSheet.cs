using Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;

namespace TitanBot2.Models
{
    public class HighScoreSheet
    {
        public List<HighScoreUser> Users = new List<HighScoreUser>();

        public HighScoreSheet(IEnumerable<ICsvLine> data)
        {
            foreach (var user in data.Skip(Configuration.Instance.HighScoreSettings.DataStartRow))
            {
                var parsed = HighScoreUser.FromCsv(user);
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

            public static HighScoreUser FromCsv(ICsvLine data)
            {
                var conf = Configuration.Instance.HighScoreSettings;
                int ranking;
                string userName;
                string clanName;
                string rankMs;
                string simMs;
                string rankRel;
                string totalRelics;
                string rawAd;
                string fullAd;

                if (!int.TryParse(data[conf.RankingCol].Trim(), out ranking))
                    return null;
                userName = data[conf.NameCol].Trim();
                clanName = data[conf.ClanCol].Trim();
                rankMs = data[conf.RankMsCol].Trim();
                simMs = data[conf.SimMSCol].Trim();
                rankRel = data[conf.RankRelCol].Trim();
                totalRelics = data[conf.TotalRelicsCol].Trim();
                rawAd = data[conf.RawADCol].Trim();
                fullAd = data[conf.FullADCol].Trim();

                return new HighScoreUser(ranking, userName, clanName, rankMs, simMs, rankRel, totalRelics, rawAd, fullAd);
            }

            public override string ToString()
            {
                return $"#{Ranking} - {UserName} [{ClanName}]";
            }
        }
    }
}
