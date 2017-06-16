using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Util;
using TT2Bot.Helpers;
using TT2Bot.Models;

namespace TT2Bot.Commands.Data
{
    [Description("Displays data about any equipment")]
    [Alias("Equip", "Equips", "Equipment")]
    class EquipmentsCommand : Command
    {
        private TT2DataService DataService { get; }

        public EquipmentsCommand(TT2DataService dataService)
        {
            DataService = dataService;
            DelayMessage = "This might take a short while, theres a fair bit of data to download!";
        }

        [Call("List")]
        [Usage("Lists all equipment for the given type")]
        async Task ListEquipmentAsync([Dense]string equipClass = null)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Name = "Equipment listing"
                },
                Color = System.Drawing.Color.LightBlue.ToDiscord(),
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = $"{BotUser.Username} Equipment tool"
                },
                Timestamp = DateTime.Now
            };
            
            IEnumerable<ValueTuple<string, string>> fields;
            List<Equipment> allEquip = await DataService.GetAllEquipment(true);
            
            switch (equipClass?.ToLower())
            {
                case "aura":
                    fields = allEquip.Where(e => e.Class == EquipmentClass.Aura)
                                     .GroupBy(e => e.BonusType)
                                     .Select(g => ValueTuple.Create(Formatter.Beautify(g.Key), string.Join("\n", g.OrderBy(e => e.Rarity))));
                    break;
                case "weapon":
                case "sword":
                    fields = allEquip.Where(e => e.Class == EquipmentClass.Weapon)
                                     .GroupBy(e => e.BonusType)
                                     .Select(g => ValueTuple.Create(Formatter.Beautify(g.Key), string.Join("\n", g.OrderBy(e => e.Rarity))));
                    break;
                case "hat":
                case "helmet":
                    fields = allEquip.Where(e => e.Class == EquipmentClass.Hat)
                                     .GroupBy(e => e.BonusType)
                                     .Select(g => ValueTuple.Create(Formatter.Beautify(g.Key), string.Join("\n", g.OrderBy(e => e.Rarity))));
                    break;
                case "slash":
                    fields = allEquip.Where(e => e.Class == EquipmentClass.Slash)
                                     .GroupBy(e => e.BonusType)
                                     .Select(g => ValueTuple.Create(Formatter.Beautify(g.Key), string.Join("\n", g.OrderBy(e => e.Rarity))));
                    break;
                case "suit":
                case "armor":
                case "body":
                    fields = allEquip.Where(e => e.Class == EquipmentClass.Suit)
                                     .GroupBy(e => e.BonusType)
                                     .Select(g => ValueTuple.Create(Formatter.Beautify(g.Key), string.Join("\n", g.OrderBy(e => e.Rarity))));
                    break;
                case "removed":
                    fields = allEquip.Where(e => e.Rarity == EquipmentRarity.Removed)
                                     .GroupBy(e => e.Class)
                                     .Select(g => ValueTuple.Create(Formatter.Beautify(g.Key), string.Join("\n", g.OrderBy(e => e.Name))));
                    break;
                default:
                    fields = null;
                    break;
            }
            if (fields == null)
                builder.WithDescription("Please use one of the following equipment types:\n" + string.Join("\n", Enum.GetNames(typeof(EquipmentClass))).Replace("None", "Removed") + $"\n\n `{Prefix}{CommandName} list [type]`");
            else
            {
                builder.WithDescription($"All {equipClass} equipment");
                foreach (var field in fields)
                    builder.AddInlineField(field.Item1, field.Item2);
            }

            await ReplyAsync("", embed: builder.Build());
        }

        EmbedBuilder GetBaseEmbed(Equipment equipment)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Equipment data for " + equipment.Name,
                    IconUrl = equipment.ImageUrl.AbsoluteUri,
                },
                ThumbnailUrl = equipment.ImageUrl.AbsoluteUri,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = $"{BotUser.Username} Equipment tool | TT2 v{equipment.FileVersion}"
                },
                Timestamp = DateTime.Now,
                Color = equipment.Image.AverageColor(0.3f, 0.5f).ToDiscord(),
            };

            builder.AddInlineField("Equipment id", equipment.Id);
            builder.AddInlineField("Equipment type", equipment.Class);
            builder.AddInlineField("Rarity", equipment.Rarity);
            builder.AddInlineField("Source", equipment.Source);

            return builder;
        }

        [Call]
        [Usage("Shows stats for a given equipment on the given level.")]
        async Task ShowEquipmentAsync([Dense] Equipment equipment, double? level = null)
        {
            var builder = GetBaseEmbed(equipment);

            if (level == null)
            {
                builder.AddField("Bonus type", Formatter.Beautify(equipment.BonusType));
                builder.AddInlineField("Bonus base", equipment.BonusType.FormatValue(equipment.BonusBase));
                builder.AddInlineField("Bonus increase", equipment.BonusType.FormatValue(equipment.BonusIncrease));
                builder.AddField("Note", "*The level displayed by equipment ingame is actually 10x lower than the real level.*");
            }
            else
            {
                builder.AddField("Bonus type", Formatter.Beautify(equipment.BonusType));
                builder.AddField($"Bonus at lv {level} (actual ~{level * 10})", equipment.BonusType.FormatValue(equipment.BonusOnLevel((int)(10 * level))));
                builder.AddField("Note", "*The level displayed by equipment ingame is actually 10x lower than the real level and rounded.*");
            }

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
