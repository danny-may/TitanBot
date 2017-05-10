using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Data
{
    public class HighScoreCommand : Command
    {
        public HighScoreCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ShowSheetAsync(1, 30));
            Calls.AddNew(a => ShowSheetAsync((int)a[0], (int)a[0]))
                 .WithArgTypes(typeof(int));
            Calls.AddNew(a => ShowSheetAsync((int)a[0], (int)a[1]))
                 .WithArgTypes(typeof(int), typeof(int));
            Alias.Add("HS");
        }

        private async Task ShowSheetAsync(int from, int to)
        {
            var sheet = await Context.TT2DataService.GetHighScores();

            IntExtensions.EnsureOrder(ref from, ref to);

            var places = Enumerable.Range(from, to - from + 1)
                                   .Select(i => sheet.Users.FirstOrDefault(u => u.Ranking == i))
                                   .ToList();

            var data = places.Select(p => new string[] { p.Ranking.ToString(), p.TotalRelics, p.UserName, p.ClanName }).ToArray();
            data = new string[][] { new string[] { "##", " Relics", " Username", " Clan" } }.Concat(data).ToArray();

            await ReplyAsync($"Here are the currently know users in rank {from} - {to}:\n```md\n{data.Tableify("[{0}]", "{0}  ")}```");

            return;
        }
    }
}
