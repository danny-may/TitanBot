using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Models.Enums;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Data
{
    public class ArtifactsCommand : Command
    {
        public ArtifactsCommand(CmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ShowArtifactAsync((Artifact)a[0]))
                 .WithArgTypes(typeof(Artifact))
                 .WithItemAsParams(0);
            Calls.AddNew(a => ShowArtifactAsync((Artifact)a[0], 1, (int)a[1]))
                 .WithArgTypes(typeof(Artifact), typeof(int))
                 .WithItemAsParams(0);
            Calls.AddNew(a => ShowArtifactAsync((Artifact)a[0], (int)a[1], (int)a[2]))
                 .WithArgTypes(typeof(Artifact), typeof(int), typeof(int))
                 .WithItemAsParams(0);
            Calls.AddNew(a => ListArtifactsAsync())
                 .WithSubCommand("List");
            Alias.Add("Art");
            Alias.Add("Arts");
            Alias.Add("Artifact");
            Usage.Add("`{0} <artifact> [from] [to]` - Shows stats for a given artifact on the given levels.");
            Usage.Add("`{0} list` - Lists all artifacts available.");
            Description = "Displays data about any artifact ";
            DelayMessage = "This might take a short while, theres a fair bit of data to download!";
        }

        private async Task ListArtifactsAsync()
        {
            var artifacts = await Context.TT2DataService.GetAllArtifacts();

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
                builder.AddInlineField($"Tier {tier.Key}", string.Join("\n", tier.Select(a => a.Name)));
            }

            await ReplyAsync("", embed: builder.Build());
        }

        private EmbedBuilder GetBaseEmbed(Artifact artifact)
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

        private async Task ShowArtifactAsync(Artifact artifact)
        {
            var builder = GetBaseEmbed(artifact);

            builder.AddInlineField("Effect type", artifact.BonusType.Beautify());
            builder.AddInlineField("Effect per Level", artifact.BonusType.FormatValue(artifact.EffectPerLevel));
            builder.AddInlineField("Effect type", BonusType.ArtifactDamage.Beautify());
            builder.AddInlineField("Effect per Level", BonusType.ArtifactDamage.FormatValue(artifact.DamageBonus));
            builder.AddInlineField("Cost Coeficient", artifact.CostCoef);
            builder.AddInlineField("Cost Exponent", artifact.CostExpo);
            builder.AddInlineField("Cost at lv 1", artifact.CostOfLevel(2) + " relics");

            await ReplyAsync("", embed: builder.Build());
        }

        private async Task ShowArtifactAsync(Artifact artifact, int from, int to)
        {
            var startLevel = Math.Min(from, to);
            var endLevel = Math.Max(from, to);

            var builder = GetBaseEmbed(artifact);

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
    }
}
