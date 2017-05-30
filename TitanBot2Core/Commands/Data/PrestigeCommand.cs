using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Data
{
    [Description("Shows you exactly what prestiging at a given point will mean for you")]
    class PrestigeCommand : Command
    {
        [Call]
        [Usage("Shows various stats about prestiging on the given stage")]
        [CallFlag(typeof(int), "b", "bos", "Uses the given BoS level")]
        [CallFlag(typeof(int), "c", "clan", "Uses the given clan level")]
        [CallFlag(typeof(int), "i", "ip", "Uses the given IP level")]
        async Task PrestigeStatsAsync(int stage)
        {
            var showIP = Flags.TryGet("i", out int ipLevel);
            var showClan = Flags.TryGet("c", out int clanLevel);
            var showBos = Flags.TryGet("b", out int bosLevel);

            ipLevel = Math.Min(20, ipLevel);
            var startingStage = (int)Math.Max(1, stage * Calculator.AdvanceStart(clanLevel));
            var totalRelics = Calculator.RelicsEarned(stage, bosLevel);
            var baseRelics = Calculator.RelicsEarned(stage, 0);
            var enemiesToKill = Enumerable.Range(startingStage, stage - startingStage).Sum(s => Calculator.TitansOnStage(s, ipLevel));
            var timeTaken = Calculator.RunTime(startingStage, stage, ipLevel, 1);
            var timeTakenSplash = Calculator.RunTime(startingStage, stage, ipLevel, 4);

            var description = "";
            if (showBos)
                description += $"\nBoS: {bosLevel}";
            if (showClan)
                description += $"\nClan: {clanLevel}";
            if (showIP)
                description += $"\nIP: {ipLevel}";

            var builder = new EmbedBuilder
            {
                Title = $"Info when prestiging on stage **{stage}**",
                Description = description,
                Color = System.Drawing.Color.Gold.ToDiscord(),
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = "Prestige data"
                }
            }
            .AddInlineField("Starting stage", startingStage)
            .AddInlineField("Relics", $"{baseRelics} + {totalRelics - baseRelics} = {totalRelics}")
            .AddInlineField("Enemies", $"{enemiesToKill} + {stage - startingStage} Bosses")
            .AddInlineField("Time", $"~{timeTaken}. 4x splash ~{timeTakenSplash}");

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
