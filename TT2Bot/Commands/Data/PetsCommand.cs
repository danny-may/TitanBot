using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Util;
using TT2Bot.Helpers;
using TT2Bot.Models;

namespace TT2Bot.Commands.Data
{
    [Description("Displays data about any pet ")]
    [Alias("Pet")]
    class PetsCommand : Command
    {
        private TT2DataService DataService { get; }

        public PetsCommand(TT2DataService dataService)
        {
            DataService = dataService;
            DelayMessage = "This might take a short while, theres a fair bit of data to download!";
        }

        EmbedBuilder GetBaseEmbed(Pet pet)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Pet data for " + pet.Name,
                    IconUrl = pet.ImageUrl.AbsoluteUri,
                },
                ThumbnailUrl = pet.ImageUrl.AbsoluteUri,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = $"{BotUser.Username} Pet tool | TT2 v{pet.FileVersion}"
                },
                Timestamp = DateTime.Now,
                Color = pet.Image.AverageColor(0.3f, 0.5f).ToDiscord(),
            };

            builder.AddInlineField("Pet id", pet.Id);

            return builder;
        }

        [Call]
        [Usage("Shows stats for a given pet on the given level")]
        async Task ShowPetAsync(Pet pet, int? level = null)
        {
            
            var builder = GetBaseEmbed(pet);

            if (level == null)
            {
                builder.AddField("Base damage", BonusType.PetDamage.FormatValue(pet.DamageBase));
                var keysOrdered = pet.IncreaseRanges.Keys.OrderBy(k => k).ToList();
                for (int i = 0; i < keysOrdered.Count(); i++)
                {
                    if (i == keysOrdered.Count() - 1)
                        builder.AddInlineField($"lv {keysOrdered[i]}+", BonusType.PetDamage.FormatValue(pet.IncreaseRanges[keysOrdered[i]]));
                    else
                        builder.AddInlineField($"lv {keysOrdered[i]}-{keysOrdered[i + 1] - 1}", BonusType.PetDamage.FormatValue(pet.IncreaseRanges[keysOrdered[i]]));
                }
                builder.AddField("Bonus type", Formatter.Beautify(pet.BonusType));
                builder.AddInlineField("Base bonus", pet.BonusType.FormatValue(pet.BonusBase));
                builder.AddInlineField("Bonus increment", pet.BonusType.FormatValue(pet.BonusIncrement));
            }
            else
            {
                var actualLevel = level ?? 1;
                builder.AddField("Bonus type", Formatter.Beautify(pet.BonusType));
                builder.AddInlineField($"Damage at lv {actualLevel}", BonusType.PetDamage.FormatValue(pet.DamageOnLevel(actualLevel)));
                builder.AddInlineField($"Bonus at lv {actualLevel}", pet.BonusType.FormatValue(pet.BonusOnLevel(actualLevel)));
                if (pet.InactiveMultiplier(actualLevel) < 1)
                {
                    builder.AddField($"Inactive % at lv {actualLevel}", Formatter.Beautify(pet.InactiveMultiplier(actualLevel) * 100) + "%");
                    builder.AddInlineField($"Inactive damage at lv {actualLevel}", BonusType.PetDamage.FormatValue(pet.InactiveMultiplier(actualLevel) * pet.DamageOnLevel(actualLevel)));
                    builder.AddInlineField($"Inactive bonus at lv {actualLevel}", pet.BonusType.FormatValue(pet.InactiveMultiplier(actualLevel) * pet.BonusOnLevel(actualLevel)));
                }
            }

            await ReplyAsync("", embed: builder.Build());
        }

        [Call("List")]
        [Usage("Lists all pets available")]
        async Task ListPetsAsync()
        {
            var pets = await DataService.GetAllPets(true);

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Name = "Pet listing"
                },
                Color = System.Drawing.Color.LightBlue.ToDiscord(),
                Description = "All Pets",
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = $"{BotUser.Username} Pet tool"
                },
                Timestamp = DateTime.Now
            };

            foreach (var bonus in pets.GroupBy(p => p.BonusType).OrderBy(t => t.Key))
            {
                builder.AddInlineField(Formatter.Beautify(bonus.Key), string.Join("\n", bonus.Select(a => $"{a.Name} ({a.Id})")));
            }

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
