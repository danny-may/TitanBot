using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TT2Bot.Models
{
    public class Pet
    {
        public int Id => _staticData.Id;
        public string Name => _staticData.Name;
        public double DamageBase { get; }
        public Dictionary<int, double> IncreaseRanges { get; }
        public BonusType BonusType { get; }
        public double BonusBase { get; }
        public double BonusIncrement { get; }
        public string FileVersion { get; }
        public Uri ImageUrl => _staticData.ImageUrl;
        public Bitmap Image { get; }

        private PetStatic _staticData { get; }

        private Pet(PetStatic staticData, double damageBase, Dictionary<int, double> increaseRanges, BonusType bonusType, double bonusBase, double bonusIncrement, Bitmap image, string version)
        {
            _staticData = staticData;
            DamageBase = damageBase;
            IncreaseRanges = increaseRanges;
            BonusType = bonusType;
            BonusBase = bonusBase;
            BonusIncrement = bonusIncrement;
            Image = image;
            FileVersion = FileVersion;
        }

        public double DamageOnLevel(int level)
        {
            return Enumerable.Range(1, level)
                             .Select(l => IncreaseRanges[IncreaseRanges.Keys.LastOrDefault(k => k <= l)])
                             .Sum() + DamageBase;
        }

        public double BonusOnLevel(int level)
        {
            return BonusBase + level * BonusIncrement;
        }

        public double InactiveMultiplier(int level)
        {
            return Math.Min(1d, (double)(level / 5) / 20);
        }

        public static PetStatic Find(string name)
        {
            var matches = All.Where(a => a.Id.ToString() == name.ToString() ||
                                          a.Name.Replace(" ", "").ToLower().Contains(name.Replace(" ", "").ToLower()) ||
                                          a.Alias.Count(v => v.ToLower() == name.ToLower()) > 0);
            if (matches.Count() != 1)
                return null;
            else
                return matches.First();
        }

        public static List<PetStatic> All { get; } = new List<PetStatic>
        {
            new PetStatic(1, "Nova", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p9.png")),
            new PetStatic(2, "Toto", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p7.png")),
            new PetStatic(3, "Cerberus ", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p2.png")),
            new PetStatic(4, "Mousy", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p5.png")),
            new PetStatic(5, "Harker ", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p1.png")),
            new PetStatic(6, "Bubbles ", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p12.png")),
            new PetStatic(7, "Demos ", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p8.png")),
            new PetStatic(8, "Tempest", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p6.png")),
            new PetStatic(9, "Basky ", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p3.png")),
            new PetStatic(10, "Scraps ", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p4.png")),
            new PetStatic(11, "Zero", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p0.png")),
            new PetStatic(12, "Polly ", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p13.png")),
            new PetStatic(13, "Hamy ", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p10.png")),
            new PetStatic(14, "Phobos ", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p11.png")),
            new PetStatic(15, "Fluffers", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p15.png")),
            new PetStatic(16, "Kit", new string[0], new Uri("http://www.cockleshell.org/static/TT2/img/p14.png")),
        };

        public class PetStatic
        {
            public int Id { get; }
            public string Name { get; }
            public string[] Alias { get; }
            public Uri ImageUrl { get; }

            internal PetStatic(int id, string name, string[] alias, Uri imageUrl)
            {
                Id = id;
                Name = name;
                Alias = alias;
                ImageUrl = imageUrl;
            }

            public Pet Build(double damageBase, Dictionary<int, double> incrementRange, BonusType bonusType, double bonusBase, double bonusIncrement, Bitmap image, string version)
            {
                return new Pet(this, damageBase, incrementRange, bonusType, bonusBase, bonusIncrement, image, version);
            }
        }
    }
}
