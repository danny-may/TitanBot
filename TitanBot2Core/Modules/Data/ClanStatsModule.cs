using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Preconditions;

namespace TitanBot2.Modules.Data
{
    public partial class DataModule
    {
        [Group("ClanStats")]
        [Summary("Shows various data for a clan of a certain level")]
        [RequireCustomPermission(0)]
        public class ClanStatsModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            public async Task ClanStatsAsync(uint clanLevel, int averageMS = 3500, int tapsPerCq = 500, params int[] attackers)
            {
                var absLevel = (int)clanLevel;

                var currentBonus = Calculator.ClanBonus(absLevel);
                var nextBonus = Calculator.ClanBonus(absLevel + 1);
                var nextTitanLordHp = Calculator.TitanLordHp(absLevel);
                var advanceStart = Calculator.AdvanceStart(absLevel);


                var builder = new EmbedBuilder
                {
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                        Text = Context.Client.CurrentUser.Username + "#" + Context.Client.CurrentUser.Discriminator
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


                await ReplyAsync("", embed: builder.Build());
            }
        }
    }
}
