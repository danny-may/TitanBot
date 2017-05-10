using Csv;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Models.Enums;

namespace TitanBot2.Services
{
    public class TT2DataService
    {
        private CachedWebClient WebClient { get; }
        private static readonly string _ghStaticUrl = "https://s3.amazonaws.com/tt2-static/info_files/";
        private static readonly string _artifactFileLocation = "/ArtifactInfo.csv";
        private static readonly string _highScoreSheet = "https://docs.google.com/spreadsheets/d/13hsvWaYvp_QGFuQ0ukcgG-FlSAj2NyW8DOvPUG3YguY/export?format=csv&id=13hsvWaYvp_QGFuQ0ukcgG-FlSAj2NyW8DOvPUG3YguY&gid=4642011";


        public TT2DataService(CachedWebClient client)
        {
            WebClient = client;
        }

        public async Task<Artifact> GetArtifact(int artifactId)
        {
            float version = 1.0f;

            Configuration.Instance.Versions.TryGetValue(_artifactFileLocation, out version);

            var data = await WebClient.GetString(_ghStaticUrl + version.ToString("#.0") + _artifactFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);
            ICsvLine artifactRow = dataCSV.SingleOrDefault(r => r[0].EndsWith("t" + artifactId));
            if (artifactRow == null)
                return null;

            int maxLevel;
            string tt1, note, name;
            BonusType bonusType;
            double effectPerLevel, damageBonus, costCoef, costExpo;

            int.TryParse(artifactRow[1], out maxLevel);
            tt1 = artifactRow[2];
            Enum.TryParse(artifactRow[3], out bonusType);
            double.TryParse(artifactRow[4], out effectPerLevel);
            double.TryParse(artifactRow[5], out damageBonus);
            double.TryParse(artifactRow[6], out costCoef);
            double.TryParse(artifactRow[7], out costExpo);
            note = artifactRow[8];
            name = artifactRow[9];

            var artifact = new Artifact(artifactId, maxLevel == 0 ? (int?)null : maxLevel, tt1, bonusType, effectPerLevel, damageBonus, costCoef, costExpo, note, name, version);

            var imageBytes = await WebClient.GetBytes(artifact.ImageUrl);

            if (imageBytes == null)
                return artifact;

            artifact.Image = new Bitmap(new MemoryStream(imageBytes));

            return artifact;
        }

        public async Task<HighScoreSheet> GetHighScores()
        {
            var data = await WebClient.GetString(_highScoreSheet, Encoding.UTF8);

            var sheet = new HighScoreSheet(CsvReader.ReadFromText(data, new CsvOptions
            {
                HeaderMode = HeaderMode.HeaderAbsent
            }));

            return sheet;
        }
    }
}
