using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Models.Enums;

namespace TitanBot2.Models
{
    public class Equipment
    {
        public string Name => _staticData.Name;
        public string Id => _staticData.Id;
        public EquipmentClass Class { get; }
        public BonusType BonusType { get; }
        public EquipmentRarity Rarity { get; }
        public double BonusBase { get; }
        public double BonusIncrease { get; }
        public EquipmentSource Source { get; }
        public string ImageUrl => _staticData.ImageUrl;
        public Bitmap Image { get; }
        public string FileVersion { get; }

        private EquipmentStatic _staticData { get; }

        internal Equipment(EquipmentStatic staticData, EquipmentClass eClass, BonusType bonusType, EquipmentRarity rarity, double bonusBase, double bonusIncrease, EquipmentSource source, Bitmap image, string fileVersion)
        {
            _staticData = staticData;
            Class = eClass;
            BonusType = bonusType;
            Rarity = rarity;
            BonusBase = bonusBase;
            BonusIncrease = bonusIncrease;
            Source = source;
            Image = image;
            FileVersion = fileVersion;
        }

        public static EquipmentStatic Find(string name)
        {
            return Equipments.SingleOrDefault(e => e.Id.ToLower() == name.ToLower() ||
                                                   e.Name.Replace(" ", "").ToLower().Contains(name.Replace(" ", "").ToLower()) ||
                                                   e.Alias.Count(v => v.ToLower() == name.ToLower()) > 0);
        }

        private static List<string> _unknownImages = new List<string>
        {
            "http://imgur.com/9BrAk1X.png",
            "http://imgur.com/7Uo6Yg0.png"
        };

        public static List<EquipmentStatic> Equipments => new List<EquipmentStatic>
        {
            new EquipmentStatic("Aura_Bats", "Bat Cave", new string[0], null),
            new EquipmentStatic("Aura_Bird", "Birds", new string[0], "http://imgur.com/tMJ0kSB.png"),
            new EquipmentStatic("Aura_BlackEnergy", "Dark Energy", new string[0], null),
            new EquipmentStatic("Aura_Bones", "Smells Fishy", new string[0], null),
            new EquipmentStatic("Aura_Bugs", "Butterfly Effect", new string[0], "http://imgur.com/kRuzQua.png"),
            new EquipmentStatic("Aura_Cards", "Magic Illusion", new string[0], "http://imgur.com/xl3SSVV.png"),
            new EquipmentStatic("Aura_Colors", "Colour Pop", new string[0], null),
            new EquipmentStatic("Aura_Diamonds", "Diamond Ring", new string[0], null),
            new EquipmentStatic("Aura_Embers", "Falling Ember", new string[0], "http://imgur.com/I0GEp2e.png"),
            new EquipmentStatic("Aura_Fire", "Flame Thrower", new string[0], null),
            new EquipmentStatic("Aura_GroundLightning", "Lightning Charge", new string[0], null),
            new EquipmentStatic("Aura_Ice", "Frost Bite", new string[0], "http://imgur.com/NanEHu1.png"),
            new EquipmentStatic("Aura_Leaves", "Leaf Shield", new string[0], null),
            new EquipmentStatic("Aura_Lightning", "Magnetic Power", new string[0], "http://imgur.com/eSVqClp.png"),
            new EquipmentStatic("Aura_Orbs", "Space Orbs", new string[0], "http://imgur.com/Y7WcAoR.png"),
            new EquipmentStatic("Aura_Star", "Star Shine", new string[0], null),
            new EquipmentStatic("Aura_Water", "Whirlpool", new string[0], null),
            new EquipmentStatic("Aura_Wind", "Blue Wind", new string[0], null),
            new EquipmentStatic("Aura_Dizzy", "Dizzy", new string[0], "http://imgur.com/SAFec8v.png"),
            new EquipmentStatic("Aura_Sparks", "Sparks", new string[0], "http://imgur.com/lgzkQxX.png"),
            new EquipmentStatic("Hat_Alien", "Alien Helmet", new string[0], "http://imgur.com/MJ3P4X7.png"),
            new EquipmentStatic("Hat_Bag", "Hat of Mystery", new string[0], null),
            new EquipmentStatic("Hat_BatGrey", "Dark Knight", new string[0], null),
            new EquipmentStatic("Hat_BatRed", "Crimson Knight", new string[0], null),
            new EquipmentStatic("Hat_Cap", "Everyday Cap", new string[0], "http://imgur.com/5lr2fOQ.png"),
            new EquipmentStatic("Hat_Cat", "Feline Helmet", new string[0], "http://imgur.com/sXfxyC8.png"),
            new EquipmentStatic("Hat_Chicken", "Chicken Beanie", new string[0], null),
            new EquipmentStatic("Hat_Football", "Football Head", new string[0], "http://imgur.com/fIJ6fx9.png"),
            new EquipmentStatic("Hat_HelmDevil", "Devil Helmet", new string[0], null),
            new EquipmentStatic("Hat_HelmHorned", "Horned Helmet", new string[0], null),
            new EquipmentStatic("Hat_HelmTall", "Magnificent Helmet", new string[0], "http://imgur.com/HCVXdGh.png"),
            new EquipmentStatic("Hat_HelmWhite", "White Warrior Helmet", new string[0], "http://imgur.com/yMm5t1u.png"),
            new EquipmentStatic("Hat_Ice", "Ice Walker Helmet", new string[0], "http://imgur.com/yTiUnfm.png"),
            new EquipmentStatic("Hat_Indie", "Fedora", new string[0], null),
            new EquipmentStatic("Hat_Ironman", "Ironguy Helmet", new string[0], "http://imgur.com/Ecrt7tb.png"),
            new EquipmentStatic("Hat_KickAss", "Castle Guard Mask", new string[0], "http://imgur.com/5GWKBx2.png"),
            new EquipmentStatic("Hat_Kid", "Whiz Kid's Cap", new string[0], null),
            new EquipmentStatic("Hat_Kitty", "Softy Kitty Hood", new string[0], null),
            new EquipmentStatic("Hat_Link", "Phrygian Cap", new string[0], "http://imgur.com/MmwZfzr.png"),
            new EquipmentStatic("Hat_Mega", "Megaguy Helmet", new string[0], null),
            new EquipmentStatic("Hat_Music", "Boom Box Head", new string[0], "http://imgur.com/XBppJFb.png"),
            new EquipmentStatic("Hat_Ninja", "Ninja Mask", new string[0], "http://imgur.com/fMLBaXG.png"),
            new EquipmentStatic("Hat_Pineapple", "Pineapple Head", new string[0], "http://imgur.com/aEFlJbC.png"),
            new EquipmentStatic("Hat_Poo", "Poop Head", new string[0], "http://imgur.com/oE671So.png"),
            new EquipmentStatic("Hat_PowerRanger", "Silver Mask", new string[0], null),
            new EquipmentStatic("Hat_Robot", "Cyclops Mask", new string[0], null),
            new EquipmentStatic("Hat_Soccer", "Soccer Head", new string[0], null),
            new EquipmentStatic("Hat_Top", "Top Hat", new string[0], null),
            new EquipmentStatic("Hat_Transformer", "Optimus Helmet", new string[0], null),
            new EquipmentStatic("Hat_Tribal", "Tribal Helmet", new string[0], "http://imgur.com/faDXew5.png"),
            new EquipmentStatic("Hat_Wizard", "Wizard Hat", new string[0], "http://imgur.com/aG4ZHZs.png"),
            new EquipmentStatic("Hat_Beanie", "Beanie", new string[0], null),
            new EquipmentStatic("Hat_Detective", "Detective Hat", new string[0], null),
            new EquipmentStatic("Hat_Ears", "Ears", new string[0], null),
            new EquipmentStatic("Hat_Halo", "Angel Halo", new string[0], null),
            new EquipmentStatic("Hat_HeadPhones", "Headphones", new string[0], null),
            new EquipmentStatic("Hat_MadHatter", "Mad Hat", new string[0], null),
            new EquipmentStatic("Hat_RedTopHat", "Red Hat", new string[0], null),
            new EquipmentStatic("Hat_Ribbon", "Ribbon", new string[0], null),
            new EquipmentStatic("Hat_Santa", "Santa", new string[0], null),
            new EquipmentStatic("Hat_SmallTook", "Small Toque", new string[0], null),
            new EquipmentStatic("Hat_Took", "Toque", new string[0], null),
            new EquipmentStatic("Slash_Darkness", "Casting Shadows", new string[0], null),
            new EquipmentStatic("Slash_Embers", "Fiery Embers", new string[0], "http://imgur.com/LOmzgve.png"),
            new EquipmentStatic("Slash_EmbersBlue", "Frosted Embers", new string[0], null),
            new EquipmentStatic("Slash_Fire", "Flames", new string[0], "http://imgur.com/rRwyJBL.png"),
            new EquipmentStatic("Slash_FireBlack", "Crimson Flames", new string[0], null),
            new EquipmentStatic("Slash_FireGray", "Gray Flames", new string[0], null),
            new EquipmentStatic("Slash_FireOrange", "Summoner Flames", new string[0], "http://imgur.com/dZx23XQ.png"),
            new EquipmentStatic("Slash_Lava", "Hot Lava", new string[0], null),
            new EquipmentStatic("Slash_Leaves", "Fallen Leaves", new string[0], null),
            new EquipmentStatic("Slash_Lightning", "Blue Lightning", new string[0], "http://imgur.com/BKy0TAN.png"),
            new EquipmentStatic("Slash_Paint", "Paint Streaks", new string[0], null),
            new EquipmentStatic("Slash_Petals", "Sakura Petals", new string[0], null),
            new EquipmentStatic("Slash_Rainbow", "Rainbow Road", new string[0], null),
            new EquipmentStatic("Slash_Smelly", "Something Stinks", new string[0], "http://imgur.com/a8aJiE0.png"),
            new EquipmentStatic("Slash_Stars", "Starry Path", new string[0], null),
            new EquipmentStatic("Slash_StarsBlue", "Glimmering Path", new string[0], null),
            new EquipmentStatic("Slash_Water", "Tidal Waves", new string[0], "http://imgur.com/j0YMECd.png"),
            new EquipmentStatic("Suit_ArmorFace", "Unknown Face", new string[0], "http://imgur.com/jorzNVD.png"),
            new EquipmentStatic("Suit_ArmorGreen", "Forest Fighter", new string[0], "http://imgur.com/0tlWmM0.png"),
            new EquipmentStatic("Suit_ArmorHighTec", "Futuristic Gladiator", new string[0], "http://imgur.com/yNWzScK.png"),
            new EquipmentStatic("Suit_ArmorIce", "Frosted Armor", new string[0], null),
            new EquipmentStatic("Suit_ArmorOrange", "Sunrise Armor", new string[0], null),
            new EquipmentStatic("Suit_ArmorPurple", "Amethyst Armor", new string[0], "http://imgur.com/Y8eW6FZ.png"),
            new EquipmentStatic("Suit_ArmorRed", "Crimson Warrior", new string[0], "http://imgur.com/wl2cRCI.png"),
            new EquipmentStatic("Suit_ArmorRoman", "Ancient Rome", new string[0], "http://imgur.com/ggAEXFp.png"),
            new EquipmentStatic("Suit_ArmorRoyal", "Fit for Royals", new string[0], "http://imgur.com/KfPCb32.png"),
            new EquipmentStatic("Suit_ArmorScale", "Scaly Suit", new string[0], "http://imgur.com/5optkvR.png"),
            new EquipmentStatic("Suit_ArmorWhite", "White Warrior", new string[0], "http://imgur.com/sWLxaZD.png"),
            new EquipmentStatic("Suit_Casual", "Everyday Sweater", new string[0], null),
            new EquipmentStatic("Suit_Chicken", "Chicken Breast", new string[0], "http://imgur.com/H0ViLsc.png"),
            new EquipmentStatic("Suit_Farmer", "Farmer Overalls", new string[0], "http://imgur.com/uExWaPm.png"),
            new EquipmentStatic("Suit_Ironman", "Ironman Suit", new string[0], "http://imgur.com/Gc1pFbQ.png"),
            new EquipmentStatic("Suit_KungFu", "Kungfu Master", new string[0], null),
            new EquipmentStatic("Suit_Ninja", "Ninja Suit", new string[0], null),
            new EquipmentStatic("Suit_Robot", "Cyclops Body", new string[0], null),
            new EquipmentStatic("Suit_Snowman", "Snowman", new string[0], "http://imgur.com/BNvApos.png"),
            new EquipmentStatic("Suit_Wizard", "Wizard Sweater", new string[0], "http://imgur.com/671G2LT.png"),
            new EquipmentStatic("Weapon_Basic", "Average Sword", new string[0], "http://imgur.com/yExdboX.png"),
            new EquipmentStatic("Weapon_Bat", "Dark Knight Sword", new string[0], "http://imgur.com/XijKRL7.png"),
            new EquipmentStatic("Weapon_BattleAxe", "Axe of Ages", new string[0], "http://imgur.com/z3YddYc.png"),
            new EquipmentStatic("Weapon_Beam", "Lightsaber", new string[0], null),
            new EquipmentStatic("Weapon_Brute", "Crimson Sword", new string[0], "http://imgur.com/C3AYhZ6.png"),
            new EquipmentStatic("Weapon_Buster", "Buster Blade", new string[0], null),
            new EquipmentStatic("Weapon_Carrot", "Carrot Stick", new string[0], "http://imgur.com/rW5ZKsc.png"),
            new EquipmentStatic("Weapon_Cleaver", "Conqueror's Cleaver", new string[0], "http://imgur.com/Kg1RGvm.png"),
            new EquipmentStatic("Weapon_Club", "Club", new string[0], null),
            new EquipmentStatic("Weapon_Excalibur", "Excalibur", new string[0], null),
            new EquipmentStatic("Weapon_Fish", "Fish Stick", new string[0], null),
            new EquipmentStatic("Weapon_Gear", "Geared Blade", new string[0], null),
            new EquipmentStatic("Weapon_Glaive", "Ancient Glaive", new string[0], null),
            new EquipmentStatic("Weapon_Halberd", "Onyx Halberd", new string[0], "http://imgur.com/JYxI4ub.png"),
            new EquipmentStatic("Weapon_Hammer", "Titansmasher", new string[0], "http://imgur.com/reCEkyl.png"),
            new EquipmentStatic("Weapon_Holy", "Knightfall Sword", new string[0], "http://imgur.com/4wVptHb.png"),
            new EquipmentStatic("Weapon_Katana", "Reforged Katana", new string[0], null),
            new EquipmentStatic("Weapon_Laser", "Energy Sword", new string[0], null),
            new EquipmentStatic("Weapon_Ninja", "Ninja Dagger", new string[0], null),
            new EquipmentStatic("Weapon_Pencil", "Blue Pencil", new string[0], "http://imgur.com/WVHeDIu.png"),
            new EquipmentStatic("Weapon_PitchFork", "Pitchfork", new string[0], null),
            new EquipmentStatic("Weapon_Poison", "Poisonous Spellblade", new string[0], "http://imgur.com/HRkq1l7.png"),
            new EquipmentStatic("Weapon_Saw", "Sharpened Saw", new string[0], null),
            new EquipmentStatic("Weapon_Skull", "Skull-crusher Sword", new string[0], "http://imgur.com/uspbFNN.png"),
            new EquipmentStatic("Weapon_Spear", "Swift Spear", new string[0], null),
            new EquipmentStatic("Weapon_Staff", "Twilight Staff", new string[0], null),
            new EquipmentStatic("Weapon_Sythe", "Death Sythe", new string[0], null),
            new EquipmentStatic("Weapon_WoodAxe", "Lumberjack Axe", new string[0], "http://imgur.com/SRmMtK3.png"),
            new EquipmentStatic("Weapon_ZigZag", "Crooked Blade", new string[0], null),
            new EquipmentStatic("Aura_Valentines", "Circle of Love", new string[0], null),
            new EquipmentStatic("Hat_Valentines", "Hat of Love", new string[0], null),
            new EquipmentStatic("Weapon_Valentines", "Heartbreaker", new string[0], "http://imgur.com/nTGH2jK.png")
        };

        public class EquipmentStatic
        {
            public string Name { get; }
            public string Id { get; }
            public string[] Alias { get; } 
            public string ImageUrl { get; }

            internal EquipmentStatic(string id, string name, string[] alias, string imageUrl)
            {
                Name = name;
                Id = id;
                Alias = alias;
                ImageUrl = imageUrl;
            }

            public Equipment BuildEquipment(EquipmentClass eClass, BonusType bonusType, EquipmentRarity rarity, double bonusBase, double bonusIncrease, EquipmentSource source, Bitmap image, string fileVersion)
            {
                return new Equipment(this, eClass, bonusType, rarity, bonusBase, bonusIncrease, source, image, fileVersion);
            }
        }
    }
}
