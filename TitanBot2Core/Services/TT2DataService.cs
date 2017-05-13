using Csv;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Models;
using TitanBot2.Models.Enums;

namespace TitanBot2.Services
{
    public class TT2DataService
    {
        private CachedWebClient WebClient { get; }
        private static readonly string _ghStaticUrl = "https://s3.amazonaws.com/tt2-static/info_files/";
        private static readonly string _artifactFileLocation = "/ArtifactInfo.csv";
        private static readonly string _equipmentFileLocation = "/EquipmentInfo.csv";
        private static readonly string _highScoreSheet = "https://docs.google.com/spreadsheets/d/13hsvWaYvp_QGFuQ0ukcgG-FlSAj2NyW8DOvPUG3YguY/export?format=csv&id=13hsvWaYvp_QGFuQ0ukcgG-FlSAj2NyW8DOvPUG3YguY&gid=4642011";


        public TT2DataService(CachedWebClient client)
        {
            WebClient = client;
        }

        private Artifact BuildArtifact(ICsvLine serverData, Artifact.ArtifactStatic staticData, Bitmap image, string version)
        {
            int maxLevel;
            string tt1, note, name;
            BonusType bonusType;
            double effectPerLevel, damageBonus, costCoef, costExpo;

            int.TryParse(serverData[1], out maxLevel);
            tt1 = serverData[2];
            Enum.TryParse(serverData[3], out bonusType);
            double.TryParse(serverData[4], out effectPerLevel);
            double.TryParse(serverData[5], out damageBonus);
            double.TryParse(serverData[6], out costCoef);
            double.TryParse(serverData[7], out costExpo);
            note = serverData[8];
            name = serverData[9];

            return staticData.BuildArtifact(maxLevel == 0 ? (int?)null : maxLevel, tt1, bonusType, effectPerLevel, damageBonus, costCoef, costExpo, note, image, version);
        }

        private Equipment BuildEquipment(ICsvLine serverData, Equipment.EquipmentStatic staticData, Bitmap image, string version)
        {
            EquipmentClass eClass;
            BonusType bonusType;
            EquipmentRarity rarity;
            double bonusBase, bonusIncrease;
            EquipmentSource source;

            Enum.TryParse(serverData[1], out eClass);
            Enum.TryParse(serverData[2], out bonusType);
            Enum.TryParse(serverData[3], out rarity);
            double.TryParse(serverData[4], out bonusBase);
            double.TryParse(serverData[5], out bonusIncrease);
            Enum.TryParse(serverData[6], out source);

            return staticData.BuildEquipment(eClass, bonusType, rarity, bonusBase, bonusIncrease, source, image, version);
        }

        public async Task<Artifact> GetArtifact(Artifact.ArtifactStatic artifactStatic)
        {

            Configuration.Instance.Versions.TryGetValue(_artifactFileLocation, out string version);

            version = version ?? "1.0";

            var data = await WebClient.GetString(_ghStaticUrl + version + _artifactFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);
            ICsvLine artifactRow = dataCSV.SingleOrDefault(r => r[0].EndsWith("t" + artifactStatic.Id));
            if (artifactRow == null)
                return null;

            var imageBytes = artifactStatic.ImageUrl == null ? null : await WebClient.GetBytes(artifactStatic.ImageUrl);

            var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

            return BuildArtifact(artifactRow, artifactStatic, image, version);
        }

        public async Task<Equipment> GetEquipment(Equipment.EquipmentStatic equipmentStatic)
        {

            Configuration.Instance.Versions.TryGetValue(_equipmentFileLocation, out string version);

            version = version ?? "1.0";

            var data = await WebClient.GetString(_ghStaticUrl + version + _equipmentFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);
            ICsvLine equipmentRow = dataCSV.SingleOrDefault(r => r[0] == equipmentStatic.Id);
            if (equipmentRow == null)
                return null;

            var imageBytes = equipmentStatic.ImageUrl == null ? null : await WebClient.GetBytes(equipmentStatic.ImageUrl);

            var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

            return BuildEquipment(equipmentRow, equipmentStatic, image, version);
        }

        public async Task<List<Artifact>> GetAllArtifacts()
        {
            Configuration.Instance.Versions.TryGetValue(_artifactFileLocation, out string version);

            version = version ?? "1.0";

            var data = await WebClient.GetString(_ghStaticUrl + version + _artifactFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);

            var items = new List<Tuple<Artifact.ArtifactStatic, ICsvLine, Bitmap>>();

            foreach (var art in Artifact.Artifacts)
            {
                var imageBytes = art.ImageUrl == null ? null : await WebClient.GetBytes(art.ImageUrl);

                var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

                var artifactRow = dataCSV.SingleOrDefault(r => r[0].EndsWith("t" + art.Id));
                if (artifactRow == null)
                    continue;

                items.Add(Tuple.Create(art, artifactRow, image));
            }

            return items.Select(i => BuildArtifact(i.Item2, i.Item1, i.Item3, version)).ToList();
        }

        public async Task<List<Equipment>> GetAllEquipment()
        {
            Configuration.Instance.Versions.TryGetValue(_equipmentFileLocation, out string version);

            version = version ?? "1.0";

            var data = await WebClient.GetString(_ghStaticUrl + version + _equipmentFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);

            var items = new List<Tuple<Equipment.EquipmentStatic, ICsvLine, Bitmap>>();

            foreach (var equip in Equipment.Equipments)
            {
                var imageBytes = equip.ImageUrl == null ? null : await WebClient.GetBytes(equip.ImageUrl);

                var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

                var equipmentRow = dataCSV.SingleOrDefault(r => r[0] == equip.Id);

                if (equipmentRow == null)
                    continue;

                items.Add(Tuple.Create(equip, equipmentRow, image));
            }

            return items.Select(i => BuildEquipment(i.Item2, i.Item1, i.Item3, version)).ToList();
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
