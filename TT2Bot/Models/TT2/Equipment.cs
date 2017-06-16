using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TT2Bot.Models
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
        public Uri ImageUrl => _staticData.ImageUrl;
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

        public double BonusOnLevel(int level)
        {
            return BonusBase + BonusIncrease * level;
        }

        public static EquipmentStatic Find(string name)
        {
            var matches = All.Where(e => e.Id.ToLower() == name.ToLower() ||
                                                e.Name.Replace(" ", "").ToLower().Contains(name.Replace(" ", "").ToLower()) ||
                                                e.Alias.Count(v => v.ToLower() == name.ToLower()) > 0);
            if (matches.Count() != 1)
                return null;
            else
                return matches.First();
        }

        private static List<Uri> _defaultImages = new List<Uri>
        {
            new Uri("http://imgur.com/lUW3IPT.png"),
            new Uri("http://imgur.com/j2SK2JH.png"),
            new Uri("http://imgur.com/wFcmlES.png"),
            new Uri("http://imgur.com/RJdU8Dj.png"),
            new Uri("http://imgur.com/YC5doBb.png"),
            new Uri("http://imgur.com/38v4GMV.png"),
            new Uri("http://imgur.com/J0M2hGl.png"),
        };

        public static List<EquipmentStatic> All { get; } = new List<EquipmentStatic>
        {
            new EquipmentStatic("Aura_Bats", "Bat Cave", new string[0], new Uri("http://i.imgur.com/6JQGW3V.png")),
            new EquipmentStatic("Aura_Bird", "Birds", new string[0], new Uri("http://i.imgur.com/iIC7MeB.png")),
            new EquipmentStatic("Aura_BlackEnergy", "Dark Energy", new string[0], new Uri("http://i.imgur.com/coFoXPO.png")),
            new EquipmentStatic("Aura_Bones", "Smells Fishy", new string[0], new Uri("http://i.imgur.com/Z7PytUd.png")),
            new EquipmentStatic("Aura_Bugs", "Butterfly Effect", new string[0], new Uri("http://i.imgur.com/ntNAFxR.png")),
            new EquipmentStatic("Aura_Cards", "Magic Illusion", new string[0], new Uri("http://i.imgur.com/tBFW0zU.png")),
            new EquipmentStatic("Aura_Colors", "Colour Pop", new string[0], new Uri("http://i.imgur.com/tpB3DKc.png")),
            new EquipmentStatic("Aura_Diamonds", "Diamond Ring", new string[0], new Uri("http://i.imgur.com/iilQvNH.png")),
            new EquipmentStatic("Aura_Embers", "Falling Ember", new string[0], new Uri("http://i.imgur.com/kyKpotc.png")),
            new EquipmentStatic("Aura_Fire", "Flame Thrower", new string[0], new Uri("http://i.imgur.com/xzhcDlg.png")),
            new EquipmentStatic("Aura_GroundLightning", "Lightning Charge", new string[0], new Uri("http://i.imgur.com/di6vXmL.png")),
            new EquipmentStatic("Aura_Ice", "Frost Bite", new string[0], new Uri("http://i.imgur.com/bK5vNwx.png")),
            new EquipmentStatic("Aura_Leaves", "Leaf Shield", new string[0], new Uri("http://i.imgur.com/oGEO2fa.png")),
            new EquipmentStatic("Aura_Lightning", "Magnetic Power", new string[0], new Uri("http://i.imgur.com/jbFiSs8.png")),
            new EquipmentStatic("Aura_Orbs", "Space Orbs", new string[0], new Uri("http://i.imgur.com/73V8eiy.png")),
            new EquipmentStatic("Aura_Star", "Star Shine", new string[0], new Uri("http://i.imgur.com/FbFL1sJ.png")),
            new EquipmentStatic("Aura_Water", "Whirlpool", new string[0], new Uri("http://i.imgur.com/LhT8Ljj.png")),
            new EquipmentStatic("Aura_Wind", "Blue Wind", new string[0], new Uri("http://i.imgur.com/J61AR1Q.png")),
            new EquipmentStatic("Aura_Dizzy", "Dizzy", new string[0], new Uri("http://i.imgur.com/WJG8MJW.png")),
            new EquipmentStatic("Aura_Sparks", "Sparks", new string[0], new Uri("http://i.imgur.com/YEN6i09.png")),
            new EquipmentStatic("Hat_Alien", "Alien Helmet", new string[0], new Uri("http://i.imgur.com/i8XIeqb.png")),
            new EquipmentStatic("Hat_Bag", "Hat of Mystery", new string[0], new Uri("http://i.imgur.com/bbRCeSq.png")),
            new EquipmentStatic("Hat_BatGrey", "Dark Knight", new string[0], new Uri("http://i.imgur.com/rjcCUwM.png")),
            new EquipmentStatic("Hat_BatRed", "Crimson Knight", new string[0], new Uri("http://i.imgur.com/PbvtRuE.png")),
            new EquipmentStatic("Hat_Cap", "Everyday Cap", new string[0], new Uri("http://i.imgur.com/S2shK39.png")),
            new EquipmentStatic("Hat_Cat", "Feline Helmet", new string[0], new Uri("http://i.imgur.com/HwKnbnI.png")),
            new EquipmentStatic("Hat_Chicken", "Chicken Beanie", new string[0], new Uri("http://i.imgur.com/nf87Btz.png")),
            new EquipmentStatic("Hat_Football", "Football Head", new string[0], new Uri("http://i.imgur.com/Ynx0btx.png")),
            new EquipmentStatic("Hat_HelmDevil", "Devil Helmet", new string[0], new Uri("http://i.imgur.com/tNc4yOa.png")),
            new EquipmentStatic("Hat_HelmHorned", "Horned Helmet", new string[0], new Uri("http://i.imgur.com/sfCcQ0H.png")),
            new EquipmentStatic("Hat_HelmTall", "Magnificent Helmet", new string[0], new Uri("http://i.imgur.com/iVON03I.png")),
            new EquipmentStatic("Hat_HelmWhite", "White Warrior Helmet", new string[0], new Uri("http://i.imgur.com/MwZHghp.png")),
            new EquipmentStatic("Hat_Ice", "Ice Walker Helmet", new string[0], new Uri("http://i.imgur.com/KzpyO7H.png")),
            new EquipmentStatic("Hat_Indie", "Fedora", new string[0], new Uri("http://i.imgur.com/UFMgJnU.png")),
            new EquipmentStatic("Hat_Ironman", "Ironguy Helmet", new string[0], new Uri("http://i.imgur.com/VZ2ubL2.png")),
            new EquipmentStatic("Hat_KickAss", "Castle Guard Mask", new string[0], new Uri("http://i.imgur.com/UeuRVEA.png")),
            new EquipmentStatic("Hat_Kid", "Whiz Kid's Cap", new string[0], new Uri("http://i.imgur.com/RQ71eNa.png")),
            new EquipmentStatic("Hat_Kitty", "Softy Kitty Hood", new string[0], new Uri("http://i.imgur.com/Re6PnpZ.png")),
            new EquipmentStatic("Hat_Link", "Phrygian Cap", new string[0], new Uri("http://i.imgur.com/hxb0ZbK.png")),
            new EquipmentStatic("Hat_Mega", "Megaguy Helmet", new string[0], new Uri("http://i.imgur.com/3ajJaNm.png")),
            new EquipmentStatic("Hat_Music", "Boom Box Head", new string[0], new Uri("http://i.imgur.com/7TKeUjt.png")),
            new EquipmentStatic("Hat_Ninja", "Ninja Mask", new string[0], new Uri("http://i.imgur.com/3JjGYku.png")),
            new EquipmentStatic("Hat_Pineapple", "Pineapple Head", new string[0], new Uri("http://i.imgur.com/TTwmbqd.png")),
            new EquipmentStatic("Hat_Poo", "Poop Head", new string[0], new Uri("http://i.imgur.com/2NdDUNZ.png")),
            new EquipmentStatic("Hat_PowerRanger", "Silver Mask", new string[0], new Uri("http://i.imgur.com/5kr3Icd.png")),
            new EquipmentStatic("Hat_Robot", "Cyclops Mask", new string[0], new Uri("http://i.imgur.com/3OvMXoX.png")),
            new EquipmentStatic("Hat_Soccer", "Soccer Head", new string[0], new Uri("http://i.imgur.com/bgl0Fcu.png")),
            new EquipmentStatic("Hat_Top", "Top Hat", new string[0], new Uri("http://i.imgur.com/G57jlHn.png")),
            new EquipmentStatic("Hat_Transformer", "Optimus Helmet", new string[0], new Uri("http://i.imgur.com/H5r8wa9.png")),
            new EquipmentStatic("Hat_Tribal", "Tribal Helmet", new string[0], new Uri("http://i.imgur.com/aVN3Tix.png")),
            new EquipmentStatic("Hat_Wizard", "Wizard Hat", new string[0], new Uri("http://i.imgur.com/MmXcXJL.png")),
            new EquipmentStatic("Hat_Beanie", "Beanie", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_Detective", "Detective Hat", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_Ears", "Ears", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_Halo", "Angel Halo", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_HeadPhones", "Headphones", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_MadHatter", "Mad Hat", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_RedTopHat", "Red Hat", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_Ribbon", "Ribbon", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_Santa", "Santa", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_SmallTook", "Small Toque", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Hat_Took", "Toque", new string[0], new Uri("http://i.imgur.com/J0M2hGl.png")),
            new EquipmentStatic("Slash_Darkness", "Casting Shadows", new string[0], new Uri("http://i.imgur.com/myS7iUT.png")),
            new EquipmentStatic("Slash_Embers", "Fiery Embers", new string[0], new Uri("http://i.imgur.com/nq8Aq26.png")),
            new EquipmentStatic("Slash_EmbersBlue", "Frosted Embers", new string[0], new Uri("http://i.imgur.com/YvWfrqF.png")),
            new EquipmentStatic("Slash_Fire", "Flames", new string[0], new Uri("http://i.imgur.com/cATztqa.png")),
            new EquipmentStatic("Slash_FireBlack", "Crimson Flames", new string[0], new Uri("http://i.imgur.com/hlbXF8Y.png")),
            new EquipmentStatic("Slash_FireGray", "Gray Flames", new string[0], new Uri("http://i.imgur.com/SPvqjt5.png")),
            new EquipmentStatic("Slash_FireOrange", "Summoner Flames", new string[0], new Uri("http://i.imgur.com/2y4R7m0.png")),
            new EquipmentStatic("Slash_Lava", "Hot Lava", new string[0], new Uri("http://i.imgur.com/NI0nmjm.png")),
            new EquipmentStatic("Slash_Leaves", "Fallen Leaves", new string[0], new Uri("http://i.imgur.com/YmfPd3H.png")),
            new EquipmentStatic("Slash_Lightning", "Blue Lightning", new string[0], new Uri("http://i.imgur.com/kWp2h7F.png")),
            new EquipmentStatic("Slash_Paint", "Paint Streaks", new string[0], new Uri("http://i.imgur.com/xgpUBxT.png")),
            new EquipmentStatic("Slash_Petals", "Sakura Petals", new string[0], new Uri("http://i.imgur.com/mOEIQam.png")),
            new EquipmentStatic("Slash_Rainbow", "Rainbow Road", new string[0], new Uri("http://i.imgur.com/mD91f1Y.png")),
            new EquipmentStatic("Slash_Smelly", "Something Stinks", new string[0], new Uri("http://i.imgur.com/alOrEsp.png")),
            new EquipmentStatic("Slash_Stars", "Starry Path", new string[0], new Uri("http://i.imgur.com/Cb4b5GV.png")),
            new EquipmentStatic("Slash_StarsBlue", "Glimmering Path", new string[0], new Uri("http://i.imgur.com/rwMyYIo.png")),
            new EquipmentStatic("Slash_Water", "Tidal Waves", new string[0], new Uri("http://i.imgur.com/VMVVmyk.png")),
            new EquipmentStatic("Suit_ArmorFace", "Unknown Face", new string[0], new Uri("http://i.imgur.com/dSOIAVN.png")),
            new EquipmentStatic("Suit_ArmorGreen", "Forest Fighter", new string[0], new Uri("http://i.imgur.com/HCulNSv.png")),
            new EquipmentStatic("Suit_ArmorHighTec", "Futuristic Gladiator", new string[0], new Uri("http://i.imgur.com/qttmnrm.png")),
            new EquipmentStatic("Suit_ArmorIce", "Frosted Armor", new string[0], new Uri("http://i.imgur.com/GVk79M2.png")),
            new EquipmentStatic("Suit_ArmorOrange", "Sunrise Armor", new string[0], new Uri("http://i.imgur.com/BxPamOw.png")),
            new EquipmentStatic("Suit_ArmorPurple", "Amethyst Armor", new string[0], new Uri("http://i.imgur.com/nRn6ZkG.png")),
            new EquipmentStatic("Suit_ArmorRed", "Crimson Warrior", new string[0], new Uri("http://i.imgur.com/3roDWVb.png")),
            new EquipmentStatic("Suit_ArmorRoman", "Ancient Rome", new string[0], new Uri("http://i.imgur.com/3IuMG7V.png")),
            new EquipmentStatic("Suit_ArmorRoyal", "Fit for Royals", new string[0], new Uri("http://i.imgur.com/WNgeKy6.png")),
            new EquipmentStatic("Suit_ArmorScale", "Scaly Suit", new string[0], new Uri("http://i.imgur.com/MD6BB8l.png")),
            new EquipmentStatic("Suit_ArmorWhite", "White Warrior", new string[0], new Uri("http://i.imgur.com/C8CPnxF.png")),
            new EquipmentStatic("Suit_Casual", "Everyday Sweater", new string[0], new Uri("http://i.imgur.com/X3JY6NF.png")),
            new EquipmentStatic("Suit_Chicken", "Chicken Breast", new string[0], new Uri("http://i.imgur.com/H9ZJoZh.png")),
            new EquipmentStatic("Suit_Farmer", "Farmer Overalls", new string[0], new Uri("http://i.imgur.com/gg7dNn1.png")),
            new EquipmentStatic("Suit_Ironman", "Ironman Suit", new string[0], new Uri("http://i.imgur.com/qn1WUvV.png")),
            new EquipmentStatic("Suit_KungFu", "Kungfu Master", new string[0], new Uri("http://i.imgur.com/a4c0za8.png")),
            new EquipmentStatic("Suit_Ninja", "Ninja Suit", new string[0], new Uri("http://i.imgur.com/mXucQ7T.png")),
            new EquipmentStatic("Suit_Robot", "Cyclops Body", new string[0], new Uri("http://i.imgur.com/T621x6N.png")),
            new EquipmentStatic("Suit_Snowman", "Snowman", new string[0], new Uri("http://i.imgur.com/4N8qygD.png")),
            new EquipmentStatic("Suit_Wizard", "Wizard Sweater", new string[0], new Uri("http://i.imgur.com/0FFKVox.png")),
            new EquipmentStatic("Weapon_Basic", "Average Sword", new string[0], new Uri("http://i.imgur.com/cvghMNg.png")),
            new EquipmentStatic("Weapon_Bat", "Dark Knight Sword", new string[0], new Uri("http://i.imgur.com/VZxcuFm.png")),
            new EquipmentStatic("Weapon_BattleAxe", "Axe of Ages", new string[0], new Uri("http://i.imgur.com/J3axNpt.png")),
            new EquipmentStatic("Weapon_Beam", "Lightsaber", new string[0], new Uri("http://i.imgur.com/ZTPQFcG.png")),
            new EquipmentStatic("Weapon_Brute", "Crimson Sword", new string[0], new Uri("http://i.imgur.com/KFu0dV7.png")),
            new EquipmentStatic("Weapon_Buster", "Buster Blade", new string[0], new Uri("http://i.imgur.com/nbFpycv.png")),
            new EquipmentStatic("Weapon_Carrot", "Carrot Stick", new string[0], new Uri("http://i.imgur.com/wVvI8Wo.png")),
            new EquipmentStatic("Weapon_Cleaver", "Conqueror's Cleaver", new string[0], new Uri("http://i.imgur.com/RuRKt2X.png")),
            new EquipmentStatic("Weapon_Club", "Club", new string[0], new Uri("http://i.imgur.com/PDleVcS.png")),
            new EquipmentStatic("Weapon_Excalibur", "Excalibur", new string[0], new Uri("http://i.imgur.com/CSLTTu5.png")),
            new EquipmentStatic("Weapon_Fish", "Fish Stick", new string[0], new Uri("http://i.imgur.com/vssvZuE.png")),
            new EquipmentStatic("Weapon_Gear", "Geared Blade", new string[0], new Uri("http://i.imgur.com/ZZ7wDya.png")),
            new EquipmentStatic("Weapon_Glaive", "Ancient Glaive", new string[0], new Uri("http://i.imgur.com/LALxySs.png")),
            new EquipmentStatic("Weapon_Halberd", "Onyx Halberd", new string[0], new Uri("http://i.imgur.com/kWCa4TW.png")),
            new EquipmentStatic("Weapon_Hammer", "Titansmasher", new string[0], new Uri("http://i.imgur.com/LIvAVnl.png")),
            new EquipmentStatic("Weapon_Holy", "Knightfall Sword", new string[0], new Uri("http://i.imgur.com/PNc1aGL.png")),
            new EquipmentStatic("Weapon_Katana", "Reforged Katana", new string[0], new Uri("http://i.imgur.com/ER0jD17.png")),
            new EquipmentStatic("Weapon_Laser", "Energy Sword", new string[0], new Uri("http://i.imgur.com/5ME9LEY.png")),
            new EquipmentStatic("Weapon_Ninja", "Ninja Dagger", new string[0], new Uri("http://i.imgur.com/xzdPZwM.png")),
            new EquipmentStatic("Weapon_Pencil", "Blue Pencil", new string[0], new Uri("http://i.imgur.com/M7iR3xz.png")),
            new EquipmentStatic("Weapon_PitchFork", "Pitchfork", new string[0], new Uri("http://i.imgur.com/A3nkxmX.png")),
            new EquipmentStatic("Weapon_Poison", "Poisonous Spellblade", new string[0], new Uri("http://i.imgur.com/kXSqN3U.png")),
            new EquipmentStatic("Weapon_Saw", "Sharpened Saw", new string[0], new Uri("http://i.imgur.com/JNYmyKi.png")),
            new EquipmentStatic("Weapon_Skull", "Skull-crusher Sword", new string[0], new Uri("http://i.imgur.com/DD8ZgRD.png")),
            new EquipmentStatic("Weapon_Spear", "Swift Spear", new string[0], new Uri("http://i.imgur.com/pixzDKe.png")),
            new EquipmentStatic("Weapon_Staff", "Twilight Staff", new string[0], new Uri("http://i.imgur.com/QXG7Tt9.png")),
            new EquipmentStatic("Weapon_Sythe", "Death Sythe", new string[0], new Uri("http://i.imgur.com/d6uxNMl.png")),
            new EquipmentStatic("Weapon_WoodAxe", "Lumberjack Axe", new string[0], new Uri("http://i.imgur.com/VHLCk4w.png")),
            new EquipmentStatic("Weapon_ZigZag", "Crooked Blade", new string[0], new Uri("http://i.imgur.com/Nj5pcMX.png")),
            new EquipmentStatic("Aura_Valentines", "Circle of Love", new string[0], new Uri("http://i.imgur.com/7WjHT6V.png")),
            new EquipmentStatic("Hat_Valentines", "Hat of Love", new string[0], new Uri("http://i.imgur.com/KNAkgcU.png")),
            new EquipmentStatic("Weapon_Valentines", "Heartbreaker", new string[0], new Uri("http://i.imgur.com/LnE4Erc.png"))
        };

        public override string ToString()
        {
            return $"{Name} ({Rarity}) [{Id}]" + (ImageUrl == null ? " - *No Image*" : "");
        }

        public class EquipmentStatic
        {
            public string Name { get; }
            public string Id { get; }
            public string[] Alias { get; } 
            public Uri ImageUrl { get; }

            internal EquipmentStatic(string id, string name, string[] alias, Uri imageUrl)
            {
                Name = name;
                Id = id;
                Alias = alias;
                ImageUrl = imageUrl;
            }

            public Equipment Build(EquipmentClass eClass, BonusType bonusType, EquipmentRarity rarity, double bonusBase, double bonusIncrease, EquipmentSource source, Bitmap image, string fileVersion)
            {
                return new Equipment(this, eClass, bonusType, rarity, bonusBase, bonusIncrease, source, image, fileVersion);
            }
        }
    }
}
