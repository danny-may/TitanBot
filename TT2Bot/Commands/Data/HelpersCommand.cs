using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Util;
using TT2Bot.Helpers;
using TT2Bot.Models;

namespace TT2Bot.Commands.Data
{
    [Description("Displays data about any hero")]
    [RequireOwner]
    [Alias("Hero")]
    class HelpersCommand : Command
    {
        private TT2DataService DataService { get; }

        public HelpersCommand(TT2DataService dataService)
        {
            DataService = dataService;
            DelayMessage = "This might take a short while, theres a fair bit of data to download!";
        }

        [Call("List")]
        [Usage("Lists all heros available")]
        
        async Task ListHelpersAsync([CallFlag('g', "group", "Groups the heroes by damage")]bool shouldGroup = false)
        {
            var helpers = await DataService.GetAllHelpers(true);

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Name = "Hero listing"
                },
                Color = System.Drawing.Color.LightBlue.ToDiscord(),
                Description = "All Heros",
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = $"{BotUser.Username} Hero tool"
                },
                Timestamp = DateTime.Now
            };

            if (shouldGroup)
                builder.AddField("Current Heroes", string.Join("\n", helpers.Where(h => h.IsInGame)
                                                                            .OrderBy(h => h.Order)
                                                                            .Select(h => $"{h.Name} - {h.HelperType.ToString().First()}")));
            else
                foreach (var group in helpers.Where(h => h.IsInGame)
                                             .OrderBy(h => h.Order)
                                             .GroupBy(h => h.HelperType))
                {
                    builder.AddField($"Current {group.Key} Heroes", "```\n" + group.Select(h => new string[] { h.Name, Formatter.Beautify(h.BaseCost) + " gold"})
                                                                         .ToArray()
                                                                         .Tableify()+"\n```");
                }

            await ReplyAsync("", embed: builder.Build());
        }

        EmbedBuilder GetBaseEmbed(Helper helper)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Hero data for " + helper.Name,
                    IconUrl = helper.ImageUrl.AbsoluteUri,
                },
                ThumbnailUrl = helper.ImageUrl.AbsoluteUri,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = $"{BotUser.Username} Hero tool | TT2 v{helper.FileVersion}"
                },
                Timestamp = DateTime.Now,
                Color = helper.Image.AverageColor(0.3f, 0.5f).ToDiscord(),
            };

            builder.AddInlineField("Hero id", helper.Id);
            builder.AddInlineField("Damage type", helper.HelperType);

            return builder;
        }

        [Call]
        [Usage("Shows stats for a given hero on the given level")]
        async Task ShowHelperAsync([Dense]Helper helper, int? level = null)
        {
            var builder = GetBaseEmbed(helper);

            if (level == null)
            {
                builder.AddInlineField("Base cost", Formatter.Beautify(helper.GetCost(0)));
                builder.AddInlineField("Base damage", Formatter.Beautify(helper.GetDps(1)));
            }
            else
            {
                builder.AddInlineField("Cost", Formatter.Beautify(helper.GetCost(0, level ?? 1)));
                builder.AddInlineField("Damage", Formatter.Beautify(helper.GetDps(level ?? 1)));
            }

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
