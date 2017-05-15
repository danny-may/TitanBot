using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Models.Enums;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Data
{
    public class PetsCommand : Command
    {
        public PetsCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ShowPetAsync((Pet)a[0]))
                 .WithArgTypes(typeof(Pet))
                 .WithItemAsParams(0);
            Calls.AddNew(a => ShowPetAsync((Pet)a[0], (int)a[1]))
                 .WithArgTypes(typeof(Pet), typeof(int))
                 .WithItemAsParams(0);
            Calls.AddNew(a => ListPetsAsync())
                 .WithSubCommand("List");
            Alias.Add("Pet");
            Usage.Add("`{0} <pet> [level]` - Shows stats for a given pet on the given level");
            Usage.Add("`{0} list` - Lists all pets available.");
            Description = "Displays data about any pet ";
            DelayMessage = "This might take a short while, theres a fair bit of data to download!";
        }

        private EmbedBuilder GetBaseEmbed(Pet pet)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Pet data for " + pet.Name,
                    IconUrl = pet.ImageUrl,
                },
                ThumbnailUrl = pet.ImageUrl,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Pet tool | TT2 v{pet.FileVersion}"
                },
                Timestamp = DateTime.Now,
                Color = pet.Image.AverageColor(0.3f, 0.5f).ToDiscord(),
            };

            builder.AddInlineField("Pet id", pet.Id);

            return builder;
        }

        private async Task ShowPetAsync(Pet pet)
        {
            var builder = GetBaseEmbed(pet);

            builder.AddField("Base damage", BonusType.PetDamage.FormatValue(pet.DamageBase));
            var keysOrdered = pet.IncreaseRanges.Keys.OrderBy(k => k).ToList();
            for (int i = 0; i < keysOrdered.Count(); i++)
            {
                if (i == keysOrdered.Count() - 1)
                    builder.AddInlineField($"lv {keysOrdered[i]}+", BonusType.PetDamage.FormatValue(pet.IncreaseRanges[keysOrdered[i]]));
                else
                    builder.AddInlineField($"lv {keysOrdered[i]}-{keysOrdered[i + 1] - 1}", BonusType.PetDamage.FormatValue(pet.IncreaseRanges[keysOrdered[i]]));
            }
            builder.AddField("Bonus type", pet.BonusType.Beautify());
            builder.AddInlineField("Base bonus", pet.BonusType.FormatValue(pet.BonusBase));
            builder.AddInlineField("Bonus increment", pet.BonusType.FormatValue(pet.BonusIncrement));

            await ReplyAsync("", embed: builder.Build());
        }

        private async Task ShowPetAsync(Pet pet, int level)
        {
            var builder = GetBaseEmbed(pet);
            builder.AddField("Bonus type", pet.BonusType.Beautify());
            builder.AddInlineField($"Damage at lv {level}", BonusType.PetDamage.FormatValue(pet.DamageOnLevel(level)));
            builder.AddInlineField($"Bonus at lv {level}", pet.BonusType.FormatValue(pet.BonusOnLevel(level)));
            if (pet.InactiveMultiplier(level) < 1)
            {
                builder.AddField($"Inactive % at lv {level}", (pet.InactiveMultiplier(level) * 100).Beautify() + "%");
                builder.AddInlineField($"Inactive damage at lv {level}", BonusType.PetDamage.FormatValue(pet.InactiveMultiplier(level) * pet.DamageOnLevel(level)));
                builder.AddInlineField($"Inactive bonus at lv {level}", pet.BonusType.FormatValue(pet.InactiveMultiplier(level) * pet.BonusOnLevel(level)));
            }

            await ReplyAsync("", embed: builder.Build());
        }

        private async Task ListPetsAsync()
        {
            var pets = await Context.TT2DataService.GetAllPets();

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Name = "Pet listing"
                },
                Color = System.Drawing.Color.LightBlue.ToDiscord(),
                Description = "All Pets",
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Pet tool"
                },
                Timestamp = DateTime.Now
            };

            foreach (var bonus in pets.GroupBy(p => p.BonusType).OrderBy(t => t.Key))
            {
                builder.AddField(bonus.Key.Beautify(), string.Join("\n", bonus.Select(a => a.Name)));
            }

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
