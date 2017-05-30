using Discord;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.General
{
    [Description("Displays some technical information about me")]
    class InfoCommand : Command
    {
        [Call]
        [Usage("Displays the info")]
        async Task ShowInfoAsync()
        {
            var guildCount = Context.Client.Guilds.Count;
            var channelCount = Context.Client.Guilds.Sum(g => g.Channels.Count);
            var userCount = Context.Client.Guilds.SelectMany(g => g.Users).Distinct().Count();
            var ramAvailable = PerformanceMonitor.getAvailableRAM();
            var cpuUsage = PerformanceMonitor.getCurrentCPUUsage();
            var timersActive = await Context.Database.Timers.GetActive();
            DateTime startTime;
            long ramUsage;
            using (var proc = Process.GetCurrentProcess())
            {
                startTime = proc.StartTime;
                ramUsage = proc.PrivateMemorySize64;
            }

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Res.Emoji.Information_source,
                    Name = "Current execution statistics"
                },
                Color = System.Drawing.Color.LawnGreen.ToDiscord(),
                Footer = new EmbedFooterBuilder
                {
                    Text = "System data for " + Context.Client.CurrentUser.Username
                },
                Timestamp = DateTime.Now
            };

            builder.AddInlineField("Guilds", guildCount.ToString())
                   .AddInlineField("Channels", channelCount.ToString())
                   .AddInlineField("Users", userCount.ToString())
                   .AddInlineField("Loaded commands", Context.CommandService.Commands.Count)
                   .AddInlineField("Loaded calls", Context.CommandService.Commands.Sum(c => c.Calls.Length))
                   .AddInlineField("Commands used this session", TotalCommands)
                   .AddInlineField("Database queries", Context.Database.TotalCalls)
                   .AddInlineField("RAM", $"{((double)ramUsage / (1024 * 1024)).ToString("#0.##")} / {ramAvailable}")
                   .AddInlineField("CPU", cpuUsage)
                   .AddInlineField("Active Timers", timersActive.Count())
                   .AddInlineField("Uptime", (DateTime.Now - startTime).Beautify());

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
