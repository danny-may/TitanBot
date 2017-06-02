using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Models.Enums;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Data
{
    [Description("Displays data about any artifact")]
    [Alias("Art", "Arts", "Artifact")]
    class ArtifactsCommand : Command
    {
        public ArtifactsCommand()
        {
            DelayMessage = "This might take a short while, theres a fair bit of data to download!";
        }

        [Call("List")]
        [Usage("Lists all artifacts available.")]
        async Task ListArtifactsAsync()
        {
            var artifacts = await Context.TT2DataService.GetAllArtifacts(true);

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Name = "Artifact listing"
                },
                Color = System.Drawing.Color.LightBlue.ToDiscord(),
                Description = "All Artifacts",
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Artifact tool"
                },
                Timestamp = DateTime.Now
            };

            foreach (var tier in artifacts.GroupBy(a => a.Tier).OrderBy(t => t.Key))
            {
                builder.AddInlineField($"Tier {tier.Key}", string.Join("\n", tier.Select(a => $"{a.Name} ({a.Id})")));
            }

            await ReplyAsync("", embed: builder.Build());
        }

        EmbedBuilder GetBaseEmbed(Artifact artifact)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Artifact data for " + artifact.Name,
                    IconUrl = artifact.ImageUrl,
                },
                ThumbnailUrl = artifact.ImageUrl,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Artifact tool | TT2 v{artifact.FileVersion}"
                },
                Timestamp = DateTime.Now,
                Color = artifact.Image.AverageColor(0.3f, 0.5f).ToDiscord(),
            };

            builder.AddInlineField("Artifact id", artifact.Id);
            builder.AddInlineField("Tier", $"[{artifact.Tier}](https://redd.it/5wae4o)");
            builder.AddField("Max level", artifact.MaxLevel == null ? "∞" : artifact.MaxLevel.Value.Beautify());

            return builder;
        }

        [Call("Budget")]
        [Usage("Shows you what the maximum level you can get an artifact to is with a given relic budget")]
        Task GetBudgetAsync([Dense]Artifact artifact, double relics, int currentLevel = 0)
        {
            relics = relics.Clamp(0, double.MaxValue);
            currentLevel = currentLevel.Clamp(0, artifact.MaxLevel ?? int.MaxValue);
            return ShowArtifactAsync(artifact, currentLevel, artifact.BudgetArtifact(relics, currentLevel ));
        }

        [Call]
        [Usage("Shows stats for a given artifact on the given levels.")]
        async Task ShowArtifactAsync([Dense]Artifact artifact, int? from = null, int? to = null)
        {
            var builder = GetBaseEmbed(artifact);

            if (from == null)
            {
                builder.AddInlineField("Effect type", artifact.BonusType.Beautify());
                builder.AddInlineField("Effect per Level", artifact.BonusType.FormatValue(artifact.EffectPerLevel));
                builder.AddInlineField("Effect type", BonusType.ArtifactDamage.Beautify());
                builder.AddInlineField("Effect per Level", BonusType.ArtifactDamage.FormatValue(artifact.DamageBonus));
                builder.AddInlineField("Cost Coeficient", artifact.CostCoef);
                builder.AddInlineField("Cost Exponent", artifact.CostExpo);
                builder.AddInlineField("Cost at lv 1", artifact.CostOfLevel(2) + " relics");
            }
            else
            {
                var startLevel = Math.Min(from ?? 0, to ?? 1).Clamp(0, artifact.MaxLevel ?? int.MaxValue);
                var endLevel = Math.Max(from ?? 0, to ?? 1).Clamp(0, artifact.MaxLevel ?? int.MaxValue);

                builder.AddField("Effect type", artifact.BonusType.Beautify());
                builder.AddInlineField($"Effect at lv {startLevel}", artifact.BonusType.FormatValue(artifact.EffectAt(startLevel)));
                builder.AddInlineField($"Effect at lv {endLevel}", artifact.BonusType.FormatValue(artifact.EffectAt(endLevel)));
                builder.AddField("Effect type", BonusType.ArtifactDamage.Beautify());
                builder.AddInlineField($"Effect at lv {startLevel}", BonusType.ArtifactDamage.FormatValue(artifact.DamageAt(startLevel)));
                builder.AddInlineField($"Effect at lv {endLevel}", BonusType.ArtifactDamage.FormatValue(artifact.DamageAt(endLevel)));
                builder.AddField($"Cost for {startLevel} -> {endLevel}", ((int)artifact.CostToLevel(startLevel + 1, endLevel)).Beautify() + " relics");
                builder.AddInlineField($"Cost at {startLevel}", ((int)artifact.CostOfLevel(startLevel + 1)).Beautify() + " relics");
                builder.AddInlineField($"Cost of lv {endLevel}", ((int)artifact.CostOfLevel(endLevel)).Beautify() + " relics");
            }
            
            await ReplyAsync("", embed: builder.Build());
        }
    }
}
