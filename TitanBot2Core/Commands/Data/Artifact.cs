using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;
using TitanBot2.Extensions;
using TitanBot2.Models.Enums;

namespace TitanBot2.Commands.Data
{
    public class Artifacts : Command
    {
        public Artifacts(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
        }

        protected override async Task RunAsync()
        {
            var artifact = await ReadAndReplyError<Artifact>(0);
            if (!artifact.IsSuccess)
                return;

            if (Context.Arguments.Length == 1)
            {
                await ShowArtifact(artifact.Value);
                return;
            }

            var arg2 = await ReadAndReplyError<int>(1);
            if (!arg2.IsSuccess)
                return;

            if (Context.Arguments.Length == 2)
            {
                await ShowArtifact(artifact.Value, 1, arg2.Value);
                return;
            }

            var arg3 = await ReadAndReplyError<int>(2);
            if (!arg3.IsSuccess)
                return;

            await ShowArtifact(artifact.Value, arg2.Value, arg3.Value);
        }

        private async Task ShowArtifact(Artifact artifact)
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

        private async Task ShowArtifact(Artifact artifact, int startLevel, int endLevel)
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

        protected override async Task<CommandCheckResponse> CheckArguments()
        {
            if (Context.Arguments.Length == 0)
                return CommandCheckResponse.FromError("Not enough arguments");
            if (Context.Arguments.Length > 3)
                return CommandCheckResponse.FromError("Too many arguments");
            if (Artifact.FindArtifact(Context.Arguments[0]) == null)
                return CommandCheckResponse.FromError($"Unknown artifact `{Context.Arguments[0]}`");
            if (Context.Arguments.Length > 1 && !(await Readers.Read<int>(Context, Context.Arguments[1])).IsSuccess)
                return CommandCheckResponse.FromError($"`{Context.Arguments[1]}` is not an integer");
            if (Context.Arguments.Length > 2 && !(await Readers.Read<int>(Context, Context.Arguments[2])).IsSuccess)
                return CommandCheckResponse.FromError($"`{Context.Arguments[2]}` is not an integer");

            return CommandCheckResponse.FromSuccess();
        }
    }
}
