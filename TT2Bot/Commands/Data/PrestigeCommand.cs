using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Util;
using TT2Bot.Helpers;

namespace TT2Bot.Commands.Data
{
    [Description("Shows you exactly what prestiging at a given point will mean for you")]
    class PrestigeCommand : Command
    {
        [Call]
        [Usage("Shows various stats about prestiging on the given stage")]
        
        async Task PrestigeStatsAsync(int stage,
            [CallFlag('b', "bos", "Uses the given BoS level")] int bosLevel = -1,
            [CallFlag('c', "clan", "Uses the given clan level")] int clanLevel = -1,
            [CallFlag('i', "ip", "Uses the given IP level")] int ipLevel = -1)
        {

            ipLevel = Math.Min(20, ipLevel);
            var startingStage = (int)Math.Max(1, stage * Calculator.AdvanceStart(clanLevel.Clamp(0, int.MaxValue)));
            var totalRelics = Calculator.RelicsEarned(stage, bosLevel.Clamp(0, int.MaxValue));
            var baseRelics = Calculator.RelicsEarned(stage, 0);
            var enemiesToKill = Enumerable.Range(startingStage, stage - startingStage).Sum(s => Calculator.TitansOnStage(s, ipLevel.Clamp(0, int.MaxValue)));
            var timeTaken = Calculator.RunTime(startingStage, stage, ipLevel.Clamp(0, int.MaxValue), 1);
            var timeTakenSplash = Calculator.RunTime(startingStage, stage, ipLevel.Clamp(0, int.MaxValue), 4);

            var description = "";
            if (bosLevel > 0)
                description += $"\nBoS: {bosLevel}";
            if (clanLevel > 0)
                description += $"\nClan: {clanLevel}";
            if (ipLevel > 0)
                description += $"\nIP: {ipLevel}";

            var builder = new EmbedBuilder
            {
                Title = $"Info when prestiging on stage **{stage}**",
                Description = description,
                Color = System.Drawing.Color.Gold.ToDiscord(),
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
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
