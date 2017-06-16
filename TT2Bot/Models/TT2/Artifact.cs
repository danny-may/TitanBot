using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TT2Bot.Models
{
    public class Artifact
    {
        public int Id => _staticData.Id;
        public int? MaxLevel { get; }
        public string TT1 { get; }
        public BonusType BonusType { get; }
        public double EffectPerLevel { get; }
        public double DamageBonus { get; }
        public double CostCoef { get; }
        public double CostExpo { get; }
        public string Note { get; }
        public string Name => _staticData.Name;
        public string FileVersion { get; }
        public ArtifactTier Tier => _staticData.Tier;
        public Uri ImageUrl => _staticData.ImageUrl;
        public Bitmap Image { get; }

        private ArtifactStatic _staticData { get; }

        internal Artifact(ArtifactStatic staticData,
                          int? maxLevel, 
                          string tt1, 
                          BonusType bonusType, 
                          double effectPerLevel, 
                          double damageBonus, 
                          double costCoef, 
                          double costExpo, 
                          string note, 
                          Bitmap image,
                          string fileVersion)
        {
            _staticData = staticData;
            MaxLevel = maxLevel;
            TT1 = tt1;
            BonusType = bonusType;
            EffectPerLevel = effectPerLevel;
            DamageBonus = damageBonus;
            CostCoef = costCoef;
            CostExpo = costExpo;
            Note = note;
            Image = image;
            FileVersion = fileVersion;
        }

        public double EffectAt(int level)
        {
            return EffectPerLevel * level;
        }

        public double DamageAt(int level)
        {
            return DamageBonus * level;
        }

        public double CostOfLevel(int level)
        {
            return Math.Ceiling(CostCoef * Math.Pow(level, CostExpo));
        }

        public double CostToLevel(int finish)
        {
            return CostToLevel(0, finish);
        }

        public double CostToLevel(int start, int finish)
        {
            double total = 0;
            for (int i = start; i <= finish; i++)
            {
                total += CostOfLevel(i);
            }
            return total;
        }

        public int BudgetArtifact(double relics, int current)
        {
            var cost = CostOfLevel(current);
            if (cost > relics)
                return current - 1;
            return BudgetArtifact(relics - cost, current + 1);
        }

        public static ArtifactStatic Find(string name)
        {
            var matches = All.Where(a => a.Id.ToString() == name.ToString() ||
                                                  a.Name.Replace(" ", "").ToLower().Contains(name.Replace(" ", "").ToLower()) ||
                                                  a.Alias.Count(v => v.ToLower() == name.ToLower()) > 0);
            if (matches.Count() != 1)
                return null;
            else
                return matches.First();
        }

        public static List<ArtifactStatic> All { get; } = new List<ArtifactStatic>
        {
            new ArtifactStatic(1, "Heroic Shield", new string[]{ "HSh" }, new Uri("http://www.cockleshell.org/static/TT2/img/a4.png"), ArtifactTier.B),
            new ArtifactStatic(2, "Stone of the Valrunes", new string[]{ "SoV", "SV" }, new Uri("http://www.cockleshell.org/static/TT2/img/a38.png"), ArtifactTier.D),
            new ArtifactStatic(3, "The Arcana Cloak", new string[]{ "tAC", "AC" }, new Uri("http://www.cockleshell.org/static/TT2/img/a22.png"), ArtifactTier.A),
            new ArtifactStatic(4, "Axe of Muerte", new string[]{ "AoM", "AM" }, new Uri("http://www.cockleshell.org/static/TT2/img/a20.png"), ArtifactTier.B),
            new ArtifactStatic(5, "Invader's Shield", new string[]{ "IS" }, new Uri("http://www.cockleshell.org/static/TT2/img/a24.png"), ArtifactTier.D),
            new ArtifactStatic(6, "Elixir of Eden", new string[]{ "EoE", "EE" }, new Uri("http://www.cockleshell.org/static/TT2/img/a34.png"), ArtifactTier.E),
            new ArtifactStatic(7, "Parchment of Foresight", new string[]{ "PoF", "PF" }, new Uri("http://www.cockleshell.org/static/TT2/img/a2.png"), ArtifactTier.A),
            new ArtifactStatic(8, "Hunter's Ointment", new string[]{ "HO" }, new Uri("http://www.cockleshell.org/static/TT2/img/a33.png"), ArtifactTier.E),
            new ArtifactStatic(9, "Laborer's Pendant", new string[]{ "LP" }, new Uri("http://www.cockleshell.org/static/TT2/img/a3.png"), ArtifactTier.B),
            new ArtifactStatic(10, "Bringer of Ragnarok", new string[]{ "BoR", "BR" }, new Uri("http://www.cockleshell.org/static/TT2/img/a27.png"), ArtifactTier.E),
            new ArtifactStatic(11, "Titan's Mask", new string[]{ "TM" }, new Uri("http://www.cockleshell.org/static/TT2/img/a36.png"), ArtifactTier.E),
            new ArtifactStatic(12, "Swamp Gauntlet", new string[]{ "SG" }, new Uri("http://www.cockleshell.org/static/TT2/img/a32.png"), ArtifactTier.E),
            new ArtifactStatic(13, "Forbidden Scroll", new string[]{ "FS" }, new Uri("http://www.cockleshell.org/static/TT2/img/a30.png"), ArtifactTier.D),
            new ArtifactStatic(14, "Aegis", new string[]{ "AG" }, new Uri("http://www.cockleshell.org/static/TT2/img/a15.png"), ArtifactTier.A),
            new ArtifactStatic(15, "Ring of Fealty", new string[]{ "RoF", "RF" }, new Uri("http://www.cockleshell.org/static/TT2/img/a14.png"), ArtifactTier.B),
            new ArtifactStatic(16, "Glacial Axe", new string[]{ "GA" }, new Uri("http://www.cockleshell.org/static/TT2/img/a28.png"), ArtifactTier.E),
            new ArtifactStatic(17, "Hero's Blade", new string[]{ "HB" }, new Uri("http://www.cockleshell.org/static/TT2/img/a18.png"), ArtifactTier.B),
            new ArtifactStatic(18, "Egg of Fortune", new string[]{ "EoF", "EF" }, new Uri("http://www.cockleshell.org/static/TT2/img/a26.png"), ArtifactTier.D),
            new ArtifactStatic(19, "Chest of Contentment", new string[]{ "CoC", "CC" }, new Uri("http://www.cockleshell.org/static/TT2/img/a25.png"), ArtifactTier.D),
            new ArtifactStatic(20, "Book of Prophecy", new string[]{ "BoP", "BP" }, new Uri("http://www.cockleshell.org/static/TT2/img/a12.png"), ArtifactTier.C),
            new ArtifactStatic(21, "Divine Chalice", new string[]{ "DC" }, new Uri("http://www.cockleshell.org/static/TT2/img/a13.png"), ArtifactTier.C),
            new ArtifactStatic(22, "Book of Shadows", new string[]{ "BoS", "BS" }, new Uri("http://www.cockleshell.org/static/TT2/img/a1.png"), ArtifactTier.S),
            new ArtifactStatic(23, "Helmet of Madness", new string[]{ "HoM", "HM" }, new Uri("http://www.cockleshell.org/static/TT2/img/a19.png"), ArtifactTier.C),
            new ArtifactStatic(24, "Staff of Radiance", new string[]{ "SoR", "SR" }, new Uri("http://www.cockleshell.org/static/TT2/img/a23.png"), ArtifactTier.C),
            new ArtifactStatic(25, "Lethe Water", new string[]{ "LW" }, new Uri("http://www.cockleshell.org/static/TT2/img/a17.png"), ArtifactTier.B),
            new ArtifactStatic(26, "Heavenly Sword", new string[]{ "HSW" }, new Uri("http://www.cockleshell.org/static/TT2/img/a10.png"), ArtifactTier.B),
            new ArtifactStatic(27, "Glove of Kuma", new string[]{ "GoK", "GK" }, new Uri("http://www.cockleshell.org/static/TT2/img/a31.png"), ArtifactTier.D),
            new ArtifactStatic(28, "Amethyst Staff", new string[]{ "AS" }, new Uri("http://www.cockleshell.org/static/TT2/img/a16.png"), ArtifactTier.B),
            new ArtifactStatic(29, "Drunken Hammer", new string[]{ "DH" }, new Uri("http://www.cockleshell.org/static/TT2/img/a37.png"), ArtifactTier.E),
            new ArtifactStatic(31, "Divine Retribution", new string[]{ "DR" }, new Uri("http://www.cockleshell.org/static/TT2/img/a11.png"), ArtifactTier.B),
            new ArtifactStatic(32, "Fruit of Eden", new string[]{ "FoE", "FE" }, new Uri("http://www.cockleshell.org/static/TT2/img/a6.png"), ArtifactTier.A),
            new ArtifactStatic(33, "The Sword of Storms", new string[]{ "tSoS", "tSS", "SS" }, new Uri("http://www.cockleshell.org/static/TT2/img/a8.png"), ArtifactTier.A),
            new ArtifactStatic(34, "Charm of the Ancient", new string[]{ "CoA", "CA" }, new Uri("http://www.cockleshell.org/static/TT2/img/a7.png"), ArtifactTier.A),
            new ArtifactStatic(35, "Blade of Damocles", new string[]{ "BoD", "BD" }, new Uri("http://www.cockleshell.org/static/TT2/img/a9.png"), ArtifactTier.A),
            new ArtifactStatic(36, "Infinity Pendulum", new string[]{ "IP" }, new Uri("http://www.cockleshell.org/static/TT2/img/a35.png"), ArtifactTier.E),
            new ArtifactStatic(37, "Oak Staff", new string[]{ "OS" }, new Uri("http://www.cockleshell.org/static/TT2/img/a29.png"), ArtifactTier.E),
            new ArtifactStatic(38, "Furies Bow", new string[]{ "FB" }, new Uri("http://www.cockleshell.org/static/TT2/img/a5.png"), ArtifactTier.A),
            new ArtifactStatic(39, "Titan Spear", new string[]{ "TS" }, new Uri("http://www.cockleshell.org/static/TT2/img/a21.png"), ArtifactTier.B)
        };

        public class ArtifactStatic
        {
            public int Id { get; }
            public string Name { get; }
            public string[] Alias { get; }
            public Uri ImageUrl { get; }
            public ArtifactTier Tier { get; }
            internal ArtifactStatic(int id, string name, string[] aliases, Uri url, ArtifactTier tier)
            {
                Id = id;
                Name = name;
                Alias = aliases;
                ImageUrl = url;
                Tier = tier;
            }

            public Artifact Build(int? maxLevel,
                                  string tt1,
                                  BonusType bonusType,
                                  double effectPerLevel,
                                  double damageBonus,
                                  double costCoef,
                                  double costExpo,
                                  string note,
                                  Bitmap image,
                                  string fileVersion)
            {
                return new Artifact(this, maxLevel, tt1, bonusType, effectPerLevel, damageBonus, costCoef, costExpo, note, image, fileVersion);
            }
        }
    }
}
