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
        private static readonly string _defaultGhStaticVersion = "1.4";
        private static readonly string _artifactFileLocation = "/ArtifactInfo.csv";
        private static readonly string _equipmentFileLocation = "/EquipmentInfo.csv";
        private static readonly string _petFileLocation = "/PetInfo.csv";
        private static readonly string _helperFileLocation = "/HelperInfo.csv";
        private static readonly string _helperSkillFileLocation = "/HelperSkillInfo.csv";
        private static readonly string _highScoreSheet = "https://docs.google.com/spreadsheets/d/13hsvWaYvp_QGFuQ0ukcgG-FlSAj2NyW8DOvPUG3YguY/export?format=csv&id=13hsvWaYvp_QGFuQ0ukcgG-FlSAj2NyW8DOvPUG3YguY&gid=4642011";


        public TT2DataService(CachedWebClient client)
        {
            WebClient = client;
        }

        private Artifact BuildArtifact(ICsvLine serverData, Artifact.ArtifactStatic staticData, Bitmap image, string version)
        {
            int.TryParse(serverData[1], out int maxLevel);
            string tt1 = serverData[2];
            Enum.TryParse(serverData[3], out BonusType bonusType);
            double.TryParse(serverData[4], out double effectPerLevel);
            double.TryParse(serverData[5], out double damageBonus);
            double.TryParse(serverData[6], out double costCoef);
            double.TryParse(serverData[7], out double costExpo);
            string note = serverData[8];
            string name = serverData[9];

            return staticData.Build(maxLevel == 0 ? (int?)null : maxLevel, tt1, bonusType, effectPerLevel, damageBonus, costCoef, costExpo, note, image, version);
        }

        private Equipment BuildEquipment(ICsvLine serverData, Equipment.EquipmentStatic staticData, Bitmap image, string version)
        {
            Enum.TryParse(serverData[1], out EquipmentClass eClass);
            Enum.TryParse(serverData[2], out BonusType bonusType);
            Enum.TryParse(serverData[3], out EquipmentRarity rarity);
            double.TryParse(serverData[4], out double bonusBase);
            double.TryParse(serverData[5], out double bonusIncrease);
            Enum.TryParse(serverData[6], out EquipmentSource source);

            return staticData.Build(eClass, bonusType, rarity, bonusBase, bonusIncrease, source, image, version);
        }

        private Pet BuildPet(ICsvLine serverData, Pet.PetStatic staticData, Bitmap image, string version)
        {
            var incrementRange = new Dictionary<int, double> { };

            double.TryParse(serverData[1], out double damageBase);
            double.TryParse(serverData[2], out double inc1to40);
            double.TryParse(serverData[3], out double inc41to80);
            double.TryParse(serverData[4], out double inc80on);
            Enum.TryParse(serverData[5], out BonusType bonusType);
            double.TryParse(serverData[6], out double bonusBase);
            double.TryParse(serverData[7], out double bonusIncrement);

            incrementRange.Add(1, inc1to40);
            incrementRange.Add(41, inc41to80);
            incrementRange.Add(81, inc80on);

            return staticData.Build(damageBase, incrementRange, bonusType, bonusBase, bonusIncrement, image, version);
        }

        private Helper BuildHelper(ICsvLine serverData, List<HelperSkill> helperSkills, Helper.HelperStatic staticData, Bitmap image, string version)
        {
            int.TryParse(serverData[0].Replace("H", ""), out int helperId);
            int.TryParse(serverData[1], out int order);
            Enum.TryParse(serverData[2], out HelperType type);
            double.TryParse(serverData[3], out double baseCost);
            int.TryParse(serverData[4], out int isInGame);

            var skills = helperSkills.Where(h => h.HelperId == helperId).ToList();

            return staticData.Build(type, order, baseCost, skills, isInGame > 0, image, version);
        }

        private HelperSkill BuildHelperSkill(ICsvLine serverData, string version)
        {
            int.TryParse(serverData[0], out int skillId);
            int.TryParse(serverData[1].Replace("H", ""), out int helperId);
            var name = serverData[2];
            Enum.TryParse(serverData[3], out BonusType type);
            double.TryParse(serverData[4], out double magnitude);
            int.TryParse(serverData[5], out int requirement);

            return new HelperSkill(skillId, helperId, name, type, magnitude, requirement, version);
        }

        

        public async Task<Artifact> GetArtifact(Artifact.ArtifactStatic artifactStatic)
        {

            Configuration.Instance.Versions.TryGetValue(_artifactFileLocation, out string version);

            version = version ?? _defaultGhStaticVersion;

            var data = await WebClient.GetString(_ghStaticUrl + version + _artifactFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);
            ICsvLine artifactRow = dataCSV.SingleOrDefault(r => r[0].EndsWith("t" + artifactStatic.Id));
            if (artifactRow == null)
                return null;

            var imageBytes = artifactStatic.ImageUrl == null ? null : await WebClient.GetBytes(artifactStatic.ImageUrl, 60 * 60 * 24);

            var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

            return BuildArtifact(artifactRow, artifactStatic, image, version);
        }

        public async Task<Equipment> GetEquipment(Equipment.EquipmentStatic equipmentStatic)
        {

            Configuration.Instance.Versions.TryGetValue(_equipmentFileLocation, out string version);

            version = version ?? _defaultGhStaticVersion;

            var data = await WebClient.GetString(_ghStaticUrl + version + _equipmentFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);
            ICsvLine equipmentRow = dataCSV.SingleOrDefault(r => r[0] == equipmentStatic.Id);
            if (equipmentRow == null)
                return null;

            var imageBytes = equipmentStatic.ImageUrl == null ? null : await WebClient.GetBytes(equipmentStatic.ImageUrl, 60 * 60 * 24);

            var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

            return BuildEquipment(equipmentRow, equipmentStatic, image, version);
        }

        public async Task<Pet> GetPet(Pet.PetStatic petStatic)
        {

            Configuration.Instance.Versions.TryGetValue(_petFileLocation, out string version);

            version = version ?? _defaultGhStaticVersion;

            var data = await WebClient.GetString(_ghStaticUrl + version + _petFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);
            ICsvLine petRow = dataCSV.SingleOrDefault(r => r[0] == "Pet" + petStatic.Id);
            if (petRow == null)
                return null;

            var imageBytes = petStatic.ImageUrl == null ? null : await WebClient.GetBytes(petStatic.ImageUrl, 60 * 60 * 24);

            var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

            return BuildPet(petRow, petStatic, image, version);
        }

        public async Task<Helper> GetHelper(Helper.HelperStatic helperStatic)
        {

            Configuration.Instance.Versions.TryGetValue(_helperFileLocation, out string version);

            version = version ?? _defaultGhStaticVersion;

            var data = await WebClient.GetString(_ghStaticUrl + version + _helperFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);
            ICsvLine helperRow = dataCSV.SingleOrDefault(r => r[0] == "H" + helperStatic.Id.ToString("00"));
            if (helperRow == null)
                return null;

            var imageBytes = helperStatic.ImageUrl == null ? null : await WebClient.GetBytes(helperStatic.ImageUrl, 60*60*24);

            var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

            var skills = await GetAllHelperSkills();

            return BuildHelper(helperRow, skills, helperStatic, image, version);
        }


        public async Task<List<Artifact>> GetAllArtifacts()
        {
            Configuration.Instance.Versions.TryGetValue(_artifactFileLocation, out string version);

            version = version ?? _defaultGhStaticVersion;

            var data = await WebClient.GetString(_ghStaticUrl + version + _artifactFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);

            var items = new List<Tuple<Artifact.ArtifactStatic, ICsvLine, Bitmap>>();

            foreach (var art in Artifact.All)
            {
                var imageBytes = art.ImageUrl == null ? null : await WebClient.GetBytes(art.ImageUrl, 60 * 60 * 24);

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

            version = version ?? _defaultGhStaticVersion;

            var data = await WebClient.GetString(_ghStaticUrl + version + _equipmentFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);

            var items = new List<Tuple<Equipment.EquipmentStatic, ICsvLine, Bitmap>>();

            foreach (var equip in Equipment.All)
            {
                var imageBytes = equip.ImageUrl == null ? null : await WebClient.GetBytes(equip.ImageUrl, 60 * 60 * 24);

                var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

                var equipmentRow = dataCSV.SingleOrDefault(r => r[0] == equip.Id);

                if (equipmentRow == null)
                    continue;

                items.Add(Tuple.Create(equip, equipmentRow, image));
            }

            return items.Select(i => BuildEquipment(i.Item2, i.Item1, i.Item3, version)).ToList();
        }

        public async Task<List<Pet>> GetAllPets()
        {
            Configuration.Instance.Versions.TryGetValue(_petFileLocation, out string version);

            version = version ?? _defaultGhStaticVersion;

            var data = await WebClient.GetString(_ghStaticUrl + version + _petFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);

            var items = new List<Tuple<Pet.PetStatic, ICsvLine, Bitmap>>();

            foreach (var pet in Pet.All)
            {
                var imageBytes = pet.ImageUrl == null ? null : await WebClient.GetBytes(pet.ImageUrl, 60 * 60 * 24);

                var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

                var petRow = dataCSV.SingleOrDefault(r => r[0] == "Pet"+pet.Id);

                if (petRow == null)
                    continue;

                items.Add(Tuple.Create(pet, petRow, image));
            }

            return items.Select(i => BuildPet(i.Item2, i.Item1, i.Item3, version)).ToList();
        }

        public async Task<List<Helper>> GetAllHelpers()
        {
            Configuration.Instance.Versions.TryGetValue(_helperFileLocation, out string version);

            version = version ?? _defaultGhStaticVersion;

            var data = await WebClient.GetString(_ghStaticUrl + version + _helperFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);

            var items = new List<Tuple<Helper.HelperStatic, ICsvLine, Bitmap>>();

            foreach (var helper in Helper.All)
            {
                var imageBytes = helper.ImageUrl == null ? null : await WebClient.GetBytes(helper.ImageUrl, 60 * 60 * 24);

                var image = imageBytes == null ? new Bitmap(1, 1) : new Bitmap(new MemoryStream(imageBytes));

                var helperRow = dataCSV.SingleOrDefault(r => r[0] == "H" + helper.Id.ToString("00"));

                if (helperRow == null)
                    continue;

                items.Add(Tuple.Create(helper, helperRow, image));
            }

            var skills = await GetAllHelperSkills();

            return items.Select(i => BuildHelper(i.Item2, skills, i.Item1, i.Item3, version)).ToList();
        }

        public async Task<List<HelperSkill>> GetAllHelperSkills()
        {
            Configuration.Instance.Versions.TryGetValue(_helperSkillFileLocation, out string version);

            version = version ?? _defaultGhStaticVersion;

            var data = await WebClient.GetString(_ghStaticUrl + version + _helperSkillFileLocation);
            if (data == null)
                return null;

            var dataCSV = CsvReader.ReadFromText(data);

            return dataCSV.Select(d => BuildHelperSkill(d, version)).ToList();
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
