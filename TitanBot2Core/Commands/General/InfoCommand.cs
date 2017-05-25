using Discord;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.General
{
    public class InfoCommand : Command
    {
        public InfoCommand(CmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Description = "Displays some technical information about me";
            Usage.Add("`{0}` - Displays the info");
            Calls.AddNew(a => ShowInfoAsync());
        }

        protected async Task ShowInfoAsync()
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
                   .AddInlineField("RAM", $"{((double)ramUsage/(1024*1024)).ToString("#0.##")} / {ramAvailable}")
                   .AddInlineField("CPU", cpuUsage)
                   .AddInlineField("Active Timers", timersActive.Count())
                   .AddInlineField("Uptime", (DateTime.Now - startTime).Beautify());

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
