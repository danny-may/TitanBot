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
    public class HighScoreCommand : Command
    {
        public HighScoreCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            //Calls.AddNew(a => ShowSheetAsync(1, 20));
            //Calls.AddNew(a => ShowSheetAsync((int)a[0], (int)a[0]))
            //     .WithArgTypes(typeof(int));
            //Calls.AddNew(a => ShowSheetAsync((int)a[0], (int)a[1]))
            //     .WithArgTypes(typeof(int), typeof(int));
            //Alias.Add("HS");
        }

        private async Task ShowSheetAsync(int from, int to)
        {
            var sheet = await Context.TT2DataService.GetHighScores();

            IntExtensions.EnsureOrder(ref from, ref to);

            var places = Enumerable.Range(from, to - from + 1)
                                   .Select(i => sheet.Users.FirstOrDefault(u => u.Ranking == i));

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Res.Emoji.Information_source,
                    Name = $"High score data for ranks {from} - {to}"
                },
                Timestamp = DateTime.Now,
                Color = System.Drawing.Color.Purple.ToDiscord()
            };
            //foreach (var user in places)
            //{
            //    builder.AddField($"#{user.Ranking} - {user.UserName}", $"Clan: {user.ClanName} | Relics: {user.TotalRelics}");
            //}
            //builder.AddInlineField("#", string.Join("\n", places.Select(p => p?.Ranking.ToString() ?? "?")));
            //builder.AddInlineField("Name", string.Join("\n", places.Select(p => p?.UserName ?? "N/A")));
            //builder.AddInlineField("Relics", string.Join("\n", places.Select(p => p?.TotalRelics ?? "N/A")));
            //builder.AddInlineField("Clan", string.Join("\n", places.Select(p => p?.ClanName ?? "N/A")));

            //await ReplyAsync("", embed: builder.Build());



            return;
        }
    }
}
