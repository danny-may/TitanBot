using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Data
{
    [Description("Shows various information for any clan with given attributes")]
    class ClanStatsCommand : Command
    {        
        internal static EmbedBuilder StatsBuilder(SocketSelfUser me, int clanLevel, int averageMS, int tapsPerCq, int[] attackers)
        {
            var absLevel = Math.Abs(clanLevel);

            var currentBonus = Calculator.ClanBonus(absLevel);
            var nextBonus = Calculator.ClanBonus(absLevel + 1);
            var nextTitanLordHp = Calculator.TitanLordHp(absLevel);
            var advanceStart = Calculator.AdvanceStart(absLevel);


            var builder = new EmbedBuilder
            {
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = me.GetAvatarUrl(),
                    Text = me.Username + "#" + me.Discriminator
                },
                Color = System.Drawing.Color.DarkOrange.ToDiscord(),
                Author = new EmbedAuthorBuilder
                {
                    Name = $"Displaying stats for a level {clanLevel} clan",

                },
                Timestamp = DateTime.Now
            };
            builder.AddInlineField("Current CQ", absLevel.Beautify());
            builder.AddInlineField("Current Bonus", currentBonus.Beautify() + "%");
            builder.AddInlineField("Next Bonus", nextBonus.Beautify() + "%");
            builder.AddInlineField("Next Titan Lord HP", nextTitanLordHp.Beautify());
            builder.AddInlineField("Advance start", (advanceStart * 100).Beautify() + "%");
            attackers = attackers.Count() == 0 ? new int[] { 20, 30, 40, 50 } : attackers;
            builder.AddField($"Requirements per boss (assuming MS {averageMS} + {tapsPerCq} taps)", string.Join("\n", attackers.Select(num =>
            {
                var dmgpp = nextTitanLordHp / num;
                var attacks = Calculator.AttacksNeeded(absLevel, num, averageMS, tapsPerCq);
                var dia = Calculator.TotalAttackCost(attacks);
                return $"Attackers: {num} | Damage/person: {dmgpp.Beautify()} | Attacks: {attacks.Beautify()} | Diamonds: {dia.Beautify()}";
            })));

            return builder;
        }

        [Call]
        [CallFlag(typeof(int), "s", "stage", "Average max stage to use")]
        [CallFlag(typeof(int), "t", "taps", "Average taps to use")]
        [CallFlag(typeof(int[]), "a", "attackers", "Number of attackers to use (array)")]
        [Usage("Shows data about a clan with the given level")]
        async Task ShowStatsAsync(int clanLevel)
        {
            if (!Flags.TryGet("s", out int averageMS))
                averageMS = 4000;
            if (!Flags.TryGet("t", out int tapsPerCq))
                tapsPerCq = 500;
            if (!Flags.TryGet("a", out int[] attackers))
                attackers = new int[] { 20, 30, 40, 50 };
            await ReplyAsync("", embed: StatsBuilder(Context.Client.CurrentUser, clanLevel, averageMS, tapsPerCq, attackers).Build());
        }
    }
}
