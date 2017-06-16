using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Formatter;
using TitanBotBase.Util;
using TT2Bot.Helpers;

namespace TT2Bot.Commands.Data
{
    [Description("Shows various information for any clan with given attributes")]
    class ClanStatsCommand : Command
    {        
        internal static EmbedBuilder StatsBuilder(OutputFormatter formatter, SocketSelfUser me, int clanLevel, int averageMS, int tapsPerCq, int[] attackers)
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
            builder.AddInlineField("Current CQ", formatter.Beautify(absLevel));
            builder.AddInlineField("Current Bonus", formatter.Beautify(currentBonus) + "%");
            builder.AddInlineField("Next Bonus", formatter.Beautify(nextBonus) + "%");
            builder.AddInlineField("Next Titan Lord HP", formatter.Beautify(nextTitanLordHp));
            builder.AddInlineField("Advance start", formatter.Beautify(advanceStart * 100) + "%");
            attackers = attackers.Count() == 0 ? new int[] { 20, 30, 40, 50 } : attackers;
            builder.AddField($"Requirements per boss (assuming MS {averageMS} + {tapsPerCq} taps)", string.Join("\n", attackers.Select(num =>
            {
                var dmgpp = nextTitanLordHp / num;
                var attacks = Calculator.AttacksNeeded(absLevel, num, averageMS, tapsPerCq);
                var dia = Calculator.TotalAttackCost(attacks);
                return $"Attackers: {num} | Damage/person: {formatter.Beautify(dmgpp)} | Attacks: {formatter.Beautify(attacks)} | Diamonds: {formatter.Beautify(dia)}";
            })));

            return builder;
        }

        [Call]
        [Usage("Shows data about a clan with the given level")]
        async Task ShowStatsAsync(int clanLevel,
            [CallFlag('s', "stage", "Average max stage to use")] int averageMs = 3500,
            [CallFlag('t', "taps", "Average taps to use")] int tapsPerCQ = 500,
            [CallFlag('a', "attackers", "Number of attackers to use (array)")]int[] attackers = null)
        {
            attackers = attackers ?? new int[] { 20, 30, 40, 50 };
            await ReplyAsync("", embed: StatsBuilder(Formatter, BotUser, clanLevel, averageMs, tapsPerCQ, attackers).Build());
        }
    }
}
