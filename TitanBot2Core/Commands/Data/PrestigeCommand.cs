using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Data
{
    public class PrestigeCommand : Command
    {
        public PrestigeCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => PrestigeStatsAsync((int)a[0], 0, 0, 0))
                 .WithArgTypes(typeof(int));
            Calls.AddNew(a => PrestigeStatsAsync((int)a[0], (int)a[1], 0, 0))
                 .WithArgTypes(typeof(int), typeof(int));
            Calls.AddNew(a => PrestigeStatsAsync((int)a[0], (int)a[1], (int)a[2], 0))
                 .WithArgTypes(typeof(int), typeof(int), typeof(int));
            Calls.AddNew(a => PrestigeStatsAsync((int)a[0], (int)a[1], (int)a[2], (int)a[3]))
                 .WithArgTypes(typeof(int), typeof(int), typeof(int), typeof(int));

            Usage.Add("`{0} <stage> [bos level] [clan level] [IP level]` - Shows various stats about prestiging with the given values");
            Description = "Shows you exactly what prestiging at a given point will mean for you";
        }

        private async Task PrestigeStatsAsync(int stage, int bosLevel, int clanLevel, int ipLevel)
        {
            stage = Math.Min(3500, stage);
            ipLevel = Math.Min(20, ipLevel);
            var startingStage = (int)Math.Max(1, stage * Calculator.AdvanceStart(clanLevel));
            var totalRelics = Calculator.RelicsEarned(stage, bosLevel);
            var baseRelics = Calculator.RelicsEarned(stage, 0);
            var enemiesToKill = Enumerable.Range(startingStage, stage - startingStage).Sum(s => Calculator.TitansOnStage(s, ipLevel));
            var timeTaken = Calculator.RunTime(startingStage, stage, ipLevel, 1);
            var timeTakenSplash = Calculator.RunTime(startingStage, stage, ipLevel, 4);

            var builder = new EmbedBuilder
            {
                Title = $"Info when prestiging on stage **{stage}** with a lv **{bosLevel}** BoS while in a level **{clanLevel}** Clan with **IP {ipLevel}**",
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
