using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Models.Enums;
using TitanBot2.Preconditions;

namespace TitanBot2.Modules.Data
{
    public partial class DataModule
    {
        [Group("Artifacts"), Alias("Artifact")]
        [Summary("Displays info about a given artifact")]
        [RequireCustomPermission(0)]
        public class ArtifactModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            [Remarks("Displays data about a given artifact by id")]
            public async Task GetArtifactAsync(int artifactId)
            {
                var artifact = await Context.TT2DataService.GetArtifact(artifactId);

                if (artifact == null)
                {
                    await ReplyAsync($"{Res.Str.ErrorText} I couldnt find an artifact with that ID!");
                    return;
                }

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
                builder.AddInlineField("Notes", artifact.Note);
                builder.AddField("Max level", artifact.MaxLevel == null ? "∞" : artifact.MaxLevel.Value.Beautify());
                builder.AddInlineField("Effect type", artifact.BonusType.Beautify());
                builder.AddInlineField("Effect per Level", artifact.BonusType.FormatValue(artifact.EffectPerLevel));
                builder.AddInlineField("Effect type", BonusType.ArtifactDamage.Beautify());
                builder.AddInlineField("Effect per Level", BonusType.ArtifactDamage.FormatValue(artifact.DamageBonus));
                builder.AddInlineField("Cost Coeficient", artifact.CostCoef);
                builder.AddInlineField("Cost Exponent", artifact.CostExpo);
                builder.AddInlineField("Cost at lv 1", artifact.CostOfLevel(2) + " relics");

                await ReplyAsync("", embed: builder.Build());
            }

            [Command(RunMode = RunMode.Async)]
            [Remarks("Displays data about a given artifact by id for the given levels")]
            public async Task GetArtifactAsync(int artifactId, int startLevel, int endLevel)
            {
                var artifact = await Context.TT2DataService.GetArtifact(artifactId);

                if (artifact == null)
                {
                    await ReplyAsync($"{Res.Str.ErrorText} I couldnt find an artifact with that ID!");
                    return;
                }

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
                builder.AddInlineField("Notes", artifact.Note);
                builder.AddInlineField("Max level", artifact.MaxLevel == null ? "∞" : artifact.MaxLevel.Value.Beautify());
                builder.AddField("Effect type", artifact.BonusType.Beautify());
                builder.AddInlineField($"Effect at lv {startLevel}", artifact.BonusType.FormatValue(artifact.EffectAt(startLevel)));
                builder.AddInlineField($"Effect at lv {endLevel}", artifact.BonusType.FormatValue(artifact.EffectAt(endLevel)));
                builder.AddField("Effect type", BonusType.ArtifactDamage.Beautify());
                builder.AddInlineField($"Effect at lv {startLevel}", BonusType.ArtifactDamage.FormatValue(artifact.DamageAt(startLevel)));
                builder.AddInlineField($"Effect at lv {endLevel}", BonusType.ArtifactDamage.FormatValue(artifact.DamageAt(endLevel)));
                builder.AddField($"Cost for {startLevel} -> {endLevel}", ((int)artifact.CostToLevel(startLevel + 1, endLevel)).Beautify() + " relics");
                builder.AddInlineField($"Cost at {startLevel}", ((int)artifact.CostOfLevel(startLevel + 1)).Beautify() + " relics");
                builder.AddInlineField($"Cost of lv {endLevel}", ((int)artifact.CostOfLevel(endLevel)).Beautify() + " relics");

                await ReplyAsync("", embed: builder.Build());

            }

            [Command(RunMode = RunMode.Async)]
            [Remarks("Displays data about a given artifact by id up to the given level")]
            public async Task GetArtifactAsync(int artifactId, int endLevel)
            {
                await GetArtifactAsync(artifactId, 1, endLevel);
            }

            [Command(RunMode = RunMode.Async)]
            [Remarks("Displays data about a given artifact by name")]
            public async Task GetArtifactAsync(string artifact)
            {
                var artifactId = Artifact.FindArtifact(artifact);

                if (artifactId == null)
                    await ReplyAsync($"{Res.Str.ErrorText} I was unable to work out which artifact you meant.");
                else
                    await GetArtifactAsync(artifactId.Value);
            }

            [Command(RunMode = RunMode.Async)]
            [Remarks("Displays data about a given artifact by name up to the given level")]
            public async Task GetArtifactAsync(string artifact, int endLevel)
            {
                var artifactId = Artifact.FindArtifact(artifact);

                if (artifactId == null)
                    await ReplyAsync($"{Res.Str.ErrorText} I was unable to work out which artifact you meant.");
                else
                    await GetArtifactAsync(artifactId.Value, 1, endLevel);
            }

            [Command(RunMode = RunMode.Async)]
            [Remarks("Displays data about a given artifact by name for the given levels")]
            public async Task GetArtifactAsync(string artifact, int startLevel, int endLevel)
            {
                var artifactId = Artifact.FindArtifact(artifact);

                if (artifactId == null)
                    await ReplyAsync($"{Res.Str.ErrorText} I was unable to work out which artifact you meant.");
                else
                    await GetArtifactAsync(artifactId.Value, startLevel, endLevel);
            }
        }
    }
}
