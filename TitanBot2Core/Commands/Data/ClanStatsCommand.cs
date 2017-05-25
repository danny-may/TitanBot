using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Data
{
    public class ClanStatsCommand : Command
    {
        public ClanStatsCommand(CmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ShowStatsAsync((int)a[0]))
                 .WithArgTypes(typeof(int));
            Calls.AddNew(a => ShowStatsAsync((int)a[0], (int)a[1]))
                 .WithArgTypes(typeof(int), typeof(int));
            Calls.AddNew(a => ShowStatsAsync((int)a[0], (int)a[1], (int)a[2]))
                 .WithArgTypes(typeof(int), typeof(int), typeof(int));
            Calls.AddNew(a => ShowStatsAsync((int)a[0], (int)a[1], (int)a[2], (int[])a[3]))
                 .WithArgTypes(typeof(int), typeof(int), typeof(int), typeof(int[]));
            Usage.Add("`{0} <clanLevel> [MS] [taps] [attackers...]` - Shows data about a clan with the given level");
            Description = "Shows various information for any clan with given attributes";
        }

        public static EmbedBuilder StatsBuilder(SocketSelfUser me, int clanLevel, int averageMS = 3500, int tapsPerCq = 500, int[] attackers = null)
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
            attackers = attackers ?? new int[] { 20, 30, 40, 50 };
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

        public async Task ShowStatsAsync(int clanLevel, int averageMS = 3500, int tapsPerCq = 500, int[] attackers = null)
        {
            await ReplyAsync("", embed: StatsBuilder(Context.Client.CurrentUser, clanLevel, averageMS, tapsPerCq, attackers).Build());
        }
    }
}
