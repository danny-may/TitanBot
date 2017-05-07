using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Models.Enums;

namespace TitanBot2.Models
{
    public class Artifact
    {
        public int Id { get; }
        public int? MaxLevel { get; }
        public string TT1 { get; }
        public BonusType BonusType { get; }
        public double EffectPerLevel { get; }
        public double DamageBonus { get; }
        public double CostCoef { get; }
        public double CostExpo { get; }
        public string Note { get; }
        public string Name { get; }
        public float FileVersion { get; }
        public string ImageUrl { get { return UrlMap[Id]; } }
        public Bitmap Image { get; set; }

        public Artifact(int id, 
                        int? maxLevel, 
                        string tt1, 
                        BonusType bonusType, 
                        double effectPerLevel, 
                        double damageBonus, 
                        double costCoef, 
                        double costExpo, 
                        string note, 
                        string name, 
                        float fileVersion)
        {
            Id = id;
            MaxLevel = maxLevel;
            TT1 = tt1;
            BonusType = bonusType;
            EffectPerLevel = effectPerLevel;
            DamageBonus = damageBonus;
            CostCoef = costCoef;
            CostExpo = costExpo;
            Note = note;
            Name = name;
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

        public static int? FindArtifact(string name)
        {
            return ArtifactNames.Where(v => v.Key.Replace(" ", "").ToLower() == name.Replace(" ", "").ToLower() || v.Value.ToString() == name)
                                .Select(v => v.Value)
                                .Cast<int?>()
                                .FirstOrDefault();
        }

        private static IDictionary<int, string> UrlMap { get; } =
            new Dictionary<int, string>
            {
                { 14, "http://www.cockleshell.org/static/TT2/img/a15.png" },
                { 28, "http://www.cockleshell.org/static/TT2/img/a16.png" },
                { 4, "http://www.cockleshell.org/static/TT2/img/a20.png" },
                { 35, "http://www.cockleshell.org/static/TT2/img/a9.png" },
                { 20, "http://www.cockleshell.org/static/TT2/img/a12.png" },
                { 22, "http://www.cockleshell.org/static/TT2/img/a1.png" },
                { 10, "http://www.cockleshell.org/static/TT2/img/a27.png" },
                { 34, "http://www.cockleshell.org/static/TT2/img/a7.png" },
                { 19, "http://www.cockleshell.org/static/TT2/img/a25.png" },
                { 21, "http://www.cockleshell.org/static/TT2/img/a13.png" },
                { 31, "http://www.cockleshell.org/static/TT2/img/a11.png" },
                { 29, "http://www.cockleshell.org/static/TT2/img/a37.png" },
                { 18, "http://www.cockleshell.org/static/TT2/img/a26.png" },
                { 6, "http://www.cockleshell.org/static/TT2/img/a34.png" },
                { 13, "http://www.cockleshell.org/static/TT2/img/a30.png" },
                { 32, "http://www.cockleshell.org/static/TT2/img/a6.png" },
                { 38, "http://www.cockleshell.org/static/TT2/img/a5.png" },
                { 16, "http://www.cockleshell.org/static/TT2/img/a28.png" },
                { 27, "http://www.cockleshell.org/static/TT2/img/a31.png" },
                { 26, "http://www.cockleshell.org/static/TT2/img/a10.png" },
                { 23, "http://www.cockleshell.org/static/TT2/img/a19.png" },
                { 1, "http://www.cockleshell.org/static/TT2/img/a4.png" },
                { 17, "http://www.cockleshell.org/static/TT2/img/a18.png" },
                { 8, "http://www.cockleshell.org/static/TT2/img/a33.png" },
                { 36, "http://www.cockleshell.org/static/TT2/img/a35.png" },
                { 5, "http://www.cockleshell.org/static/TT2/img/a24.png" },
                { 9, "http://www.cockleshell.org/static/TT2/img/a3.png" },
                { 25, "http://www.cockleshell.org/static/TT2/img/a17.png" },
                { 37, "http://www.cockleshell.org/static/TT2/img/a29.png" },
                { 7, "http://www.cockleshell.org/static/TT2/img/a2.png" },
                { 15, "http://www.cockleshell.org/static/TT2/img/a14.png" },
                { 24, "http://www.cockleshell.org/static/TT2/img/a23.png" },
                { 2, "http://www.cockleshell.org/static/TT2/img/a38.png" },
                { 12, "http://www.cockleshell.org/static/TT2/img/a32.png" },
                { 3, "http://www.cockleshell.org/static/TT2/img/a22.png" },
                { 33, "http://www.cockleshell.org/static/TT2/img/a8.png" },
                { 39, "http://www.cockleshell.org/static/TT2/img/a21.png" },
                { 11, "http://www.cockleshell.org/static/TT2/img/a36.png" }
            };

        private static IDictionary<string, int> ArtifactNames { get; } =
            new Dictionary<string, int>
            {
                { "Heroic Shield", 1 },
                { "HSh", 1 },
                { "SV", 2 },
                { "SoV", 2 },
                { "Stone of the Valrunes", 2 },
                { "AC", 3 },
                { "tAC", 3 },
                { "The Arcana Cloak", 3 },
                { "AM", 4 },
                { "AoM", 4 },
                { "Axe of Muerte", 4 },
                { "Invader's Shield", 5 },
                { "IS", 5 },
                { "EE", 6 },
                { "Elixir of Eden", 6 },
                { "EoE", 6 },
                { "Parchment of Foresight", 7 },
                { "PF", 7 },
                { "PoF", 7 },
                { "HO", 8 },
                { "Hunter's Ointment", 8 },
                { "Laborer's Pendant", 9 },
                { "LP", 9 },
                { "BoR", 10 },
                { "BR", 10 },
                { "Bringer of Ragnarok", 10 },
                { "Titan's Mask", 11 },
                { "TM", 11 },
                { "SG", 12 },
                { "Swamp Gauntlet", 12 },
                { "Forbidden Scroll", 13 },
                { "FS", 13 },
                { "Aegis", 14 },
                { "AG", 14 },
                { "RF", 15 },
                { "Ring of Fealty", 15 },
                { "RoF", 15 },
                { "GA", 16 },
                { "Glacial Axe", 16 },
                { "HB", 17 },
                { "Hero's Blade", 17 },
                { "EF", 18 },
                { "Egg of Fortune", 18 },
                { "EoF", 18 },
                { "CC", 19 },
                { "Chest of Contentment", 19 },
                { "CoC", 19 },
                { "Book of Prophecy", 20 },
                { "BoP", 20 },
                { "BP", 20 },
                { "DC", 21 },
                { "Divine Chalice", 21 },
                { "Book of Shadows", 22 },
                { "BoS", 22 },
                { "BS", 22 },
                { "Helmet of Madness", 23 },
                { "HM", 23 },
                { "HoM", 23 },
                { "SoR", 24 },
                { "SR", 24 },
                { "Staff of Radiance", 24 },
                { "Lethe Water", 25 },
                { "LW", 25 },
                { "Heavenly Sword", 26 },
                { "HSW", 26 },
                { "GK", 27 },
                { "Glove of Kuma", 27 },
                { "GoK", 27 },
                { "Amethyst Staff", 28 },
                { "AS", 28 },
                { "DH", 29 },
                { "Drunken Hammer", 29 },
                { "Divine Retribution", 31 },
                { "DR", 31 },
                { "FE", 32 },
                { "FoE", 32 },
                { "Fruit of Eden", 32 },
                { "SS", 33 },
                { "The Sword of Storms", 33 },
                { "tSoS", 33 },
                { "tSS", 33 },
                { "CA", 34 },
                { "Charm of the Ancient", 34 },
                { "CoA", 34 },
                { "BD", 35 },
                { "Blade of Damocles", 35 },
                { "BoD", 35 },
                { "Infinity Pendulum", 36 },
                { "IP", 36 },
                { "Oak Staff", 37 },
                { "OS", 37 },
                { "FB", 38 },
                { "Furies Bow", 38 },
                { "Titan Spear", 39 },
                { "TS", 39 }
            };
    }
}
