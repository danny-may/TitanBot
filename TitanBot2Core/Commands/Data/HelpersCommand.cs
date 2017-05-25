using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Data
{
    public class HelpersCommand : Command
    {
        public HelpersCommand(CmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ShowHelperAsync((Helper)a[0]))
                     .WithArgTypes(typeof(Helper))
                     .WithItemAsParams(0);
            Calls.AddNew(a => ShowHelperAsync((Helper)a[0], (int)a[1]))
                 .WithArgTypes(typeof(Helper), typeof(int))
                 .WithItemAsParams(0);
            Calls.AddNew(a => ListHelpersAsync(false))
                 .WithSubCommand("List");
            Calls.AddNew(a => ListHelpersAsync(true))
                 .WithSubCommand("List", "Group");
            Alias.Add("Hero");
            Usage.Add("`{0} <hero> [level]` - Shows stats for a given hero on the given level");
            Usage.Add("`{0} list` - Lists all heros available.");
            Usage.Add("`{0} list group` - Lists all heros available, grouped by damage type.");
            Description = "Displays data about any hero";
            DelayMessage = "This might take a short while, theres a fair bit of data to download!";
            Name = "Heroes";

            RequireOwner = true;
        }

        private async Task ListHelpersAsync(bool grouped)
        {
            var helpers = await Context.TT2DataService.GetAllHelpers();

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Name = "Hero listing"
                },
                Color = System.Drawing.Color.LightBlue.ToDiscord(),
                Description = "All Heros",
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Hero tool"
                },
                Timestamp = DateTime.Now
            };

            if (!grouped)
                builder.AddField("Current Heroes", string.Join("\n", helpers.Where(h => h.IsInGame)
                                                                            .OrderBy(h => h.Order)
                                                                            .Select(h => $"{h.Name} - {h.HelperType.ToString().First()}")));
            else
                foreach (var group in helpers.Where(h => h.IsInGame)
                                             .OrderBy(h => h.Order)
                                             .GroupBy(h => h.HelperType))
                {
                    builder.AddField($"Current {group.Key} Heroes", "```\n" + group.Select(h => new string[] { h.Name, h.BaseCost.Beautify() + " gold"})
                                                                         .ToArray()
                                                                         .Tableify()+"\n```");
                }

            await ReplyAsync("", embed: builder.Build());
        }

        private EmbedBuilder GetBaseEmbed(Helper helper)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Hero data for " + helper.Name,
                    IconUrl = helper.ImageUrl,
                },
                ThumbnailUrl = helper.ImageUrl,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Hero tool | TT2 v{helper.FileVersion}"
                },
                Timestamp = DateTime.Now,
                Color = helper.Image.AverageColor(0.3f, 0.5f).ToDiscord(),
            };

            builder.AddInlineField("Hero id", helper.Id);
            builder.AddInlineField("Damage type", helper.HelperType);

            return builder;
        }

        private async Task ShowHelperAsync(Helper helper)
        {
            var builder = GetBaseEmbed(helper);

            builder.AddInlineField("Base cost", helper.GetCost(0).Beautify());
            builder.AddInlineField("Base damage", helper.GetDps(1).Beautify());

            await ReplyAsync("", embed: builder.Build());
        }

        private async Task ShowHelperAsync(Helper helper, int level)
        {
            var builder = GetBaseEmbed(helper);

            builder.AddInlineField("Cost", helper.GetCost(0, level).Beautify());
            builder.AddInlineField("Damage", helper.GetDps(level).Beautify());

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
