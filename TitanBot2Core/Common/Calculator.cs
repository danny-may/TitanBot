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
            => 150000 + Math.Pow(clanQuest - 1, 0.9) * 900000;

        public static double ClanBonus(int clanQuest)
            => 100 * 
               Math.Pow(1.1, Math.Min(clanQuest, 200)) * 
               Math.Pow(1.05, Math.Max(clanQuest - 200, 0));

        public static int AttackCost(int attackNo)
            => BossAttackCosts[Math.Min(attackNo, BossAttackCosts.Length - 1)];

        public static int TotalAttackCost(int attacks)
            => Enumerable.Range(0, attacks).Sum(v => AttackCost(v));

        public static double AdvanceStart(int clanQuest)
            => Math.Min((double)clanQuest / 1000, 0.5);


    }
}
