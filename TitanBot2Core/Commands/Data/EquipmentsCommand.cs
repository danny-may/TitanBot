using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Models.Enums;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Data
{
    public class EquipmentsCommand : Command
    {
        public EquipmentsCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ShowEquipmentAsync((Equipment)a[0]))
                 .WithArgTypes(typeof(Equipment))
                 .WithItemAsParams(0);
            Calls.AddNew(a => ShowEquipmentAsync((Equipment)a[0], (double)a[1]))
                 .WithArgTypes(typeof(Equipment), typeof(double))
                 .WithItemAsParams(0);
            Calls.AddNew(a => ListEquipmentAsync(null))
                 .WithSubCommand("List");
            Calls.AddNew(a => ListEquipmentAsync((string)a[0]))
                 .WithArgTypes(typeof(string))
                 .WithItemAsParams(0)
                 .WithSubCommand("List");
            Alias.Add("Equip");
            Alias.Add("Equips");
            Alias.Add("Equipment");
            Usage.Add("`{0} <equipment> [level]` - Shows stats for a given equipment on the given level.");
            Usage.Add("`{0} list [equipment type]` - Lists all equipment for the given type");
            Description = "Displays data about any equipment";
            DelayMessage = "This might take a short while, theres a fair bit of data to download!";
        }

        private async Task ListEquipmentAsync(string equipClass)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Name = "Equipment listing"
                },
                Color = System.Drawing.Color.LightBlue.ToDiscord(),
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Equipment tool"
                },
                Timestamp = DateTime.Now
            };

            IEnumerable<IGrouping<BonusType, Equipment>> grouped;
            List<Equipment> allEquip = await Context.TT2DataService.GetAllEquipment();
            switch (equipClass?.ToLower())
            {

                case "aura":
                    grouped = allEquip.Where(e => e.Class == EquipmentClass.Aura)
                                      .GroupBy(e => e.BonusType);
                    break;
                case "weapon":
                case "sword":
                    grouped = allEquip.Where(e => e.Class == EquipmentClass.Weapon)
                                      .GroupBy(e => e.BonusType);
                    break;
                case "hat":
                case "helmet":
                    grouped = allEquip.Where(e => e.Class == EquipmentClass.Hat)
                                      .GroupBy(e => e.BonusType);
                    break;
                case "slash":
                    grouped = allEquip.Where(e => e.Class == EquipmentClass.Slash)
                                      .GroupBy(e => e.BonusType);
                    break;
                case "suit":
                case "armor":
                case "body":
                    grouped = allEquip.Where(e => e.Class == EquipmentClass.Suit)
                                      .GroupBy(e => e.BonusType);
                    break;
                case "removed":
                    grouped = allEquip.Where(e => e.Rarity == EquipmentRarity.Removed)
                                      .GroupBy(e => e.BonusType);
                    break;
                default:
                    grouped = null;
                    break;
            }
            if (grouped == null)
                builder.WithDescription("Please use one of the following equipment types:\n" + string.Join("\n", Enum.GetNames(typeof(EquipmentClass))).Replace("None", "Removed") + $"\n\n `{Context.Prefix}{Context.Command} list <type>`");
            else
            {
                builder.WithDescription($"All {equipClass} equipment");
                foreach (var type in grouped)
                {
                    builder.AddInlineField(type.Key.Beautify(), string.Join("\n", type.OrderByDescending(e => e.Rarity)
                                                                                .Select(e => $"{e.Name} ({e.Rarity})" + (e.ImageUrl == null ? " - *No image*" : ""))));
                }
            }

            await ReplyAsync("", embed: builder.Build());
        }

        private EmbedBuilder GetBaseEmbed(Equipment equipment)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Equipment data for " + equipment.Name,
                    IconUrl = equipment.ImageUrl,
                },
                ThumbnailUrl = equipment.ImageUrl,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Equipment tool | TT2 v{equipment.FileVersion}"
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

        private async Task ShowEquipmentAsync(Equipment equipment)
        {
            var builder = GetBaseEmbed(equipment);

            builder.AddField("Bonus type", equipment.BonusType.Beautify());
            builder.AddInlineField("Bonus base", equipment.BonusType.FormatValue(equipment.BonusBase));
            builder.AddInlineField("Bonus increase", equipment.BonusType.FormatValue(equipment.BonusIncrease));
            builder.AddField("Note", "*The level displayed by equipment ingame is actually 10x lower than the real level.*");

            await ReplyAsync("", embed: builder.Build());
        }

        private async Task ShowEquipmentAsync(Equipment equipment, double level)
        {
            var builder = GetBaseEmbed(equipment);

            builder.AddField("Bonus type", equipment.BonusType.Beautify());
            builder.AddField($"Bonus at {level} (actual ~{level * 10})", equipment.BonusType.FormatValue(equipment.BonusOnLevel((int)(10 * level))));
            builder.AddField("Note", "*The level displayed by equipment ingame is actually 10x lower than the real level and rounded.*");

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
