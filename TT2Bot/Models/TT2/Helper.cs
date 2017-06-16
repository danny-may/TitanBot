using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TT2Bot.Models
{
    public class Helper
    {

        public int Id => _staticData.Id;
        public string Name => _staticData.Name;
        public int Order { get; }
        public HelperType HelperType { get; }
        public double BaseCost { get; }
        public IReadOnlyList<HelperSkill> Skills { get; }
        public bool IsInGame { get; }
        public string FileVersion { get; }
        public Uri ImageUrl => _staticData.ImageUrl;
        public Bitmap Image { get; }

        private HelperStatic _staticData { get; }

        private Helper(HelperStatic staticData, int order, HelperType type, double baseCost, List<HelperSkill> skills, bool isInGame, Bitmap image, string version)
        {
            _staticData = staticData;
            Order = order;
            HelperType = type;
            BaseCost = baseCost;
            Skills = skills.AsReadOnly();
            IsInGame = isInGame;
            Image = image;
            FileVersion = version;
        }

        public double GetDps(int level)
        {
            var efficiency = Math.Pow(1f - 0.023f * Math.Min(Order, 34), Math.Min(Order, 34));


            return BaseCost * 0.1f * efficiency * level * 1.0;
        }

        private static int[] _evolveLevels = new int[] { 1000, 2000, 3000 };

        public double GetCost(int level)
        {
            int evomult = 1;
            if (_evolveLevels.Contains(level))
                evomult = 1000;

            return Math.Ceiling(Math.Round(BaseCost * Math.Pow(1.082, (double)level), 6)) * evomult;
        }

        public double GetCost(int level, int count)
        {
            return Enumerable.Range(level, count).Sum(l => GetCost(l));
        }

        public static HelperStatic Find(string name)
        {
            var matches = All.Where(a => a.Id.ToString() == name.ToString() ||
                                         a.Name.Replace(" ", "").ToLower().Contains(name.Replace(" ", "").ToLower()) ||
                                         a.Alias.Count(v => v.ToLower() == name.ToLower()) > 0);
            if (matches.Count() != 1)
                return null;
            else
                return matches.First();
        }

        public static List<HelperStatic> All { get; } = new List<HelperStatic>
        {
            new HelperStatic(1, "Zato the Blind Staff Master ", new string[0], null),
            new HelperStatic(2, "Lance, Knight of Cobalt Steel ", new string[0], null),
            new HelperStatic(3, "Maddie, Shadow Thief ", new string[0], null),
            new HelperStatic(4, "Kronus, Bringer of Judgement", new string[0], null),
            new HelperStatic(5, "Saje the Garden Keeper ", new string[0], null),
            new HelperStatic(6, "Pingo of the Tori ", new string[0], null),
            new HelperStatic(7, "Aya the Lightning Violet ", new string[0], null),
            new HelperStatic(8, "Gulbrand the Destroyer ", new string[0], null),
            new HelperStatic(9, "Rhys Mage of Order Evetga", new string[0], null),
            new HelperStatic(10, "Kiki the Dragon Rider ", new string[0], null),
            new HelperStatic(11, "Lala Quickshot", new string[0], null),
            new HelperStatic(12, "Boomoh Doctor ", new string[0], null),
            new HelperStatic(13, "Wally Wat the Magician", new string[0], null),
            new HelperStatic(14, "Nohni the Spearit", new string[0], null),
            new HelperStatic(15, "Kin the Puffy Beast", new string[0], null),
            new HelperStatic(16, "Zolom Blaster, Space Hunter ", new string[0], null),
            new HelperStatic(17, "Princess Titania of Fay", new string[0], null),
            new HelperStatic(18, "Maya Muerta the Watcher ", new string[0], null),
            new HelperStatic(19, "Jayce the Ruthless Cutter ", new string[0], null),
            new HelperStatic(20, "Cosette, Jewel of House Sabre ", new string[0], null),
            new HelperStatic(21, "Sophia, Champion of Swords ", new string[0], null),
            new HelperStatic(22, "Lil' Ursa", new string[0], null),
            new HelperStatic(23, "Dex-1000", new string[0], null),
            new HelperStatic(24, "Rosabella Bonnie Archer ", new string[0], null),
            new HelperStatic(25, "Beany Sprout the 1st  ", new string[0], null),
            new HelperStatic(26, "Captain Davey Cannon", new string[0], null),
            new HelperStatic(27, "Sawyer the Wild Gunslinger ", new string[0], null),
            new HelperStatic(28, "Miki the Graceful Dancer ", new string[0], null),
            new HelperStatic(29, "The Great Pharaoh ", new string[0], null),
            new HelperStatic(30, "The Great Madame Cass", new string[0], null),
            new HelperStatic(31, "Jazz Rockerfellow ", new string[0], null),
            new HelperStatic(32, "Lady Lucy the Night Caster ", new string[0], null),
            new HelperStatic(33, "Finn the Funny Guard ", new string[0], null),
            new HelperStatic(34, "Maple the Autumn Guardian", new string[0], null),
            new HelperStatic(35, "Yzafa the Fearsome Bandit", new string[0], null),
            new HelperStatic(36, "Damon of the Darkness ", new string[0], null),
            new HelperStatic(37, "Mina the Priestess of Light", new string[0], null)
        };

        public class HelperStatic
        {
            public int Id { get; }
            public string Name { get; }
            public string[] Alias { get; }
            public Uri ImageUrl { get; }

            internal HelperStatic(int id, string name, string[] alias, Uri imageUrl)
            {
                Id = id;
                Name = name;
                Alias = alias;
                ImageUrl = imageUrl;
            }

            public Helper Build(HelperType type, int order, double baseCost, List<HelperSkill> skills, bool isInGame, Bitmap image, string version)
            {
                return new Helper(this, order, type, baseCost, skills, isInGame, image, version);
            }
        }
    }
}
