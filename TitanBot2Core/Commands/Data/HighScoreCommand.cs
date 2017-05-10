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
            Description = $"Shows data from the high score sheet, which can be found [here](https://docs.google.com/spreadsheets/d/13hsvWaYvp_QGFuQ0ukcgG-FlSAj2NyW8DOvPUG3YguY/pubhtml?gid=4642011cYS8TLGYU)\nAll credit to <@261814131282149377>, <@169180650203512832> and <@169915601496702977> for running the sheet!";
            Usage.Add("`{0}` - Shows the top 30 users");
            Usage.Add("`{0} <position>` - Shows the person who is at the specified postition");
            Usage.Add("`{0} <from> <to>` - Shows the positions in the range you give");

        }

        private async Task ShowSheetAsync(int from, int to)
        {
            var sheet = await Context.TT2DataService.GetHighScores();

            IntExtensions.EnsureOrder(ref from, ref to);

            var places = Enumerable.Range(from, to - from + 1)
                                   .Select(i => sheet.Users.FirstOrDefault(u => u.Ranking == i))
                                   .Where(p => p != null)
                                   .ToList();

            if (places.Count == 0)
            {
                await ReplyAsync($"There were no users for the range {from} - {to}!");
                return;
            }

            var data = places.Select(p => new string[] { p.Ranking.ToString(), p.TotalRelics, p.UserName, p.ClanName }).ToArray();
            data = new string[][] { new string[] { "##", " Relics", " Username", " Clan" } }.Concat(data).ToArray();

            await ReplyAsync($"Here are the currently know users in rank {from} - {to}:\n```md\n{data.Tableify("[{0}]", "{0}  ")}```");

            return;
        }
    }
}
