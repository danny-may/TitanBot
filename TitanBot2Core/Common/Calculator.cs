using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Common
{
    public static class Calculator
    {
        private static int[] BossAttackCosts { get; } = new int[] { 0, 5, 25, 50, 75, 100, 125, 150 };

        public static double TitanLordHp(int clanQuest)
            => 1500000 + Math.Pow(clanQuest - 1, 0.9) * 900000;

        public static double ClanBonus(int clanQuest)
            => 100 * 
               Math.Pow(1.1, Math.Min(clanQuest, 200)) * 
               Math.Pow(1.05, Math.Max(clanQuest - 200, 0));

        public static int AttackCost(int attackNo)
            => BossAttackCosts[Math.Min(attackNo, BossAttackCosts.Length - 1)];

        public static int TotalAttackCost(int attacks)
            => Enumerable.Range(0, attacks).Sum(v => AttackCost(v));

        public static double AdvanceStart(int clanQuest)
            => Math.Min(0.5, Math.Min((double)clanQuest, 200) / 1000 + Math.Max((double)clanQuest - 200, 0) / 2000);

        public static int AttacksNeeded(int clanLevel, int attackers, int maxStage, int tapsPerAttack)
            => (int)Math.Ceiling((TitanLordHp(clanLevel) / attackers) / (Math.Max(50, maxStage) * tapsPerAttack + Math.Max(50, maxStage) * 90 * 2));

        public static int RelicsEarned(int stage, int bosLevel)
            => (int)Math.Floor((1 + (double)bosLevel * 0.05) * Math.Pow((((double)stage - 75) / 14), 1.75));

        public static int TitansOnStage(int stage, int ipLevel)
            => Math.Max(1, (stage/1000) * 4 + 10 - ipLevel);

        public static TimeSpan RunTime(int from, int to, int ipLevel, int splashAmount)
            => new TimeSpan(0, 0, (int)Math.Ceiling((to - from) + Enumerable.Range(from, to - from).Sum(s => TitansOnStage(s, ipLevel) / splashAmount + TitansOnStage(s, ipLevel) % splashAmount) * 0.4));
    }
}
