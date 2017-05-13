using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TitanBot2.Models.Enums;

namespace TitanBot2.Models
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
        public string ImageUrl => _staticData.ImageUrl;
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

        public static ArtifactStatic Find(string name)
        {
            return Artifacts.SingleOrDefault(a => a.Id.ToString() == name.ToString() ||
                                                  a.Name.Replace(" ", "").ToLower().Contains(name.Replace(" ", "").ToLower()) ||
                                                  a.Alias.Count(v => v.ToLower() == name.ToLower()) > 0);
        }

        public static List<ArtifactStatic> Artifacts => new List<ArtifactStatic>
        {
            new ArtifactStatic(1, "Heroic Shield", new string[]{ "HSh" }, "http://www.cockleshell.org/static/TT2/img/a4.png", ArtifactTier.B),
            new ArtifactStatic(2, "Stone of the Valrunes", new string[]{ "SoV", "SV" }, "http://www.cockleshell.org/static/TT2/img/a38.png", ArtifactTier.D),
            new ArtifactStatic(3, "The Arcana Cloak", new string[]{ "tAC", "AC" }, "http://www.cockleshell.org/static/TT2/img/a22.png", ArtifactTier.A),
            new ArtifactStatic(4, "Axe of Muerte", new string[]{ "AoM", "AM" }, "http://www.cockleshell.org/static/TT2/img/a20.png", ArtifactTier.B),
            new ArtifactStatic(5, "Invader's Shield", new string[]{ "IS" }, "http://www.cockleshell.org/static/TT2/img/a24.png", ArtifactTier.D),
            new ArtifactStatic(6, "Elixir of Eden", new string[]{ "EoE", "EE" }, "http://www.cockleshell.org/static/TT2/img/a34.png", ArtifactTier.E),
            new ArtifactStatic(7, "Parchment of Foresight", new string[]{ "PoF", "PF" }, "http://www.cockleshell.org/static/TT2/img/a2.png", ArtifactTier.A),
            new ArtifactStatic(8, "Hunter's Ointment", new string[]{ "HO" }, "http://www.cockleshell.org/static/TT2/img/a33.png", ArtifactTier.E),
            new ArtifactStatic(9, "Laborer's Pendant", new string[]{ "LP" }, "http://www.cockleshell.org/static/TT2/img/a3.png", ArtifactTier.B),
            new ArtifactStatic(10, "Bringer of Ragnarok", new string[]{ "BoR", "BR" }, "http://www.cockleshell.org/static/TT2/img/a27.png", ArtifactTier.E),
            new ArtifactStatic(11, "Titan's Mask", new string[]{ "TM" }, "http://www.cockleshell.org/static/TT2/img/a36.png", ArtifactTier.E),
            new ArtifactStatic(12, "Swamp Gauntlet", new string[]{ "SG" }, "http://www.cockleshell.org/static/TT2/img/a32.png", ArtifactTier.E),
            new ArtifactStatic(13, "Forbidden Scroll", new string[]{ "FS" }, "http://www.cockleshell.org/static/TT2/img/a30.png", ArtifactTier.D),
            new ArtifactStatic(14, "Aegis", new string[]{ "AG" }, "http://www.cockleshell.org/static/TT2/img/a15.png", ArtifactTier.A),
            new ArtifactStatic(15, "Ring of Fealty", new string[]{ "RoF", "RF" }, "http://www.cockleshell.org/static/TT2/img/a14.png", ArtifactTier.B),
            new ArtifactStatic(16, "Glacial Axe", new string[]{ "GA" }, "http://www.cockleshell.org/static/TT2/img/a28.png", ArtifactTier.E),
            new ArtifactStatic(17, "Hero's Blade", new string[]{ "HB" }, "http://www.cockleshell.org/static/TT2/img/a18.png", ArtifactTier.B),
            new ArtifactStatic(18, "Egg of Fortune", new string[]{ "EoF", "EF" }, "http://www.cockleshell.org/static/TT2/img/a26.png", ArtifactTier.D),
            new ArtifactStatic(19, "Chest of Contentment", new string[]{ "CoC", "CC" }, "http://www.cockleshell.org/static/TT2/img/a25.png", ArtifactTier.D),
            new ArtifactStatic(20, "Book of Prophecy", new string[]{ "BoP", "BP" }, "http://www.cockleshell.org/static/TT2/img/a12.png", ArtifactTier.C),
            new ArtifactStatic(21, "Divine Chalice", new string[]{ "DC" }, "http://www.cockleshell.org/static/TT2/img/a13.png", ArtifactTier.C),
            new ArtifactStatic(22, "Book of Shadows", new string[]{ "BoS", "BS" }, "http://www.cockleshell.org/static/TT2/img/a1.png", ArtifactTier.S),
            new ArtifactStatic(23, "Helmet of Madness", new string[]{ "HoM", "HM" }, "http://www.cockleshell.org/static/TT2/img/a19.png", ArtifactTier.C),
            new ArtifactStatic(24, "Staff of Radiance", new string[]{ "SoR", "SR" }, "http://www.cockleshell.org/static/TT2/img/a23.png", ArtifactTier.C),
            new ArtifactStatic(25, "Lethe Water", new string[]{ "LW" }, "http://www.cockleshell.org/static/TT2/img/a17.png", ArtifactTier.B),
            new ArtifactStatic(26, "Heavenly Sword", new string[]{ "HSW" }, "http://www.cockleshell.org/static/TT2/img/a10.png", ArtifactTier.B),
            new ArtifactStatic(27, "Glove of Kuma", new string[]{ "GoK", "GK" }, "http://www.cockleshell.org/static/TT2/img/a31.png", ArtifactTier.D),
            new ArtifactStatic(28, "Amethyst Staff", new string[]{ "AS" }, "http://www.cockleshell.org/static/TT2/img/a16.png", ArtifactTier.B),
            new ArtifactStatic(29, "Drunken Hammer", new string[]{ "DH" }, "http://www.cockleshell.org/static/TT2/img/a37.png", ArtifactTier.E),
            new ArtifactStatic(31, "Divine Retribution", new string[]{ "DR" }, "http://www.cockleshell.org/static/TT2/img/a11.png", ArtifactTier.B),
            new ArtifactStatic(32, "Fruit of Eden", new string[]{ "FoE", "FE" }, "http://www.cockleshell.org/static/TT2/img/a6.png", ArtifactTier.A),
            new ArtifactStatic(33, "The Sword of Storms", new string[]{ "tSoS", "tSS", "SS" }, "http://www.cockleshell.org/static/TT2/img/a8.png", ArtifactTier.A),
            new ArtifactStatic(34, "Charm of the Ancient", new string[]{ "CoA", "CA" }, "http://www.cockleshell.org/static/TT2/img/a7.png", ArtifactTier.A),
            new ArtifactStatic(35, "Blade of Damocles", new string[]{ "BoD", "BD" }, "http://www.cockleshell.org/static/TT2/img/a9.png", ArtifactTier.A),
            new ArtifactStatic(36, "Infinity Pendulum", new string[]{ "IP" }, "http://www.cockleshell.org/static/TT2/img/a35.png", ArtifactTier.E),
            new ArtifactStatic(37, "Oak Staff", new string[]{ "OS" }, "http://www.cockleshell.org/static/TT2/img/a29.png", ArtifactTier.E),
            new ArtifactStatic(38, "Furies Bow", new string[]{ "FB" }, "http://www.cockleshell.org/static/TT2/img/a5.png", ArtifactTier.A),
            new ArtifactStatic(39, "Titan Spear", new string[]{ "TS" }, "http://www.cockleshell.org/static/TT2/img/a21.png", ArtifactTier.B)
        };

        public class ArtifactStatic
        {
            public int Id { get; }
            public string Name { get; }
            public string[] Alias { get; }
            public string ImageUrl { get; }
            public ArtifactTier Tier { get; }
            internal ArtifactStatic(int id, string name, string[] aliases, string url, ArtifactTier tier)
            {
                Id = id;
                Name = name;
                Alias = aliases;
                ImageUrl = url;
                Tier = tier;
            }

            public Artifact BuildArtifact(int? maxLevel,
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
