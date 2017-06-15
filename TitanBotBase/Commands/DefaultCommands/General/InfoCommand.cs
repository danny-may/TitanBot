using Discord;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Util;

namespace TitanBotBase.Commands.DefautlCommands.General
{
    [Description("Displays some technical information about me")]
    class InfoCommand : Command
    {
        [Call]
        [Usage("Displays the info")]
        async Task ShowInfoAsync()
        {
            var guildCount = Client.Guilds.Count;
            var channelCount = Client.Guilds.Sum(g => g.Channels.Count);
            var userCount = Client.Guilds.SelectMany(g => g.Users).Distinct().Count();
            var ramAvailable = PerformanceUtil.getAvailableRAM();
            var cpuUsage = PerformanceUtil.getCurrentCPUUsage();
            var timersActive = Scheduler.ActiveCount();
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
                    IconUrl = null,
                    Name = "Current execution statistics"
                },
                Color = System.Drawing.Color.LawnGreen.ToDiscord(),
                Footer = new EmbedFooterBuilder
                {
                    Text = "System data for " + BotUser.Username
                },
                Timestamp = DateTime.Now
            };

            builder.AddInlineField("Guilds", Formatter.Beautify(guildCount))
                   .AddInlineField("Channels", Formatter.Beautify(channelCount))
                   .AddInlineField("Users", Formatter.Beautify(userCount))
                   .AddInlineField("Loaded commands", Formatter.Beautify(CommandService.Commands.Count))
                   .AddInlineField("Loaded calls", Formatter.Beautify(CommandService.Commands.Sum(c => c.Calls.Count)))
                   .AddInlineField("Commands used this session", Formatter.Beautify(TotalCommands))
                   .AddInlineField("Database queries", Formatter.Beautify(Database.TotalCalls))
                   .AddInlineField("RAM", $"{Formatter.Beautify((double)ramUsage / (1024 * 1024))} / {Formatter.Beautify(ramAvailable)}")
                   .AddInlineField("CPU", Formatter.Beautify(cpuUsage))
                   .AddInlineField("Active Timers", Formatter.Beautify(timersActive))
                   .AddInlineField("Uptime", Formatter.Beautify(DateTime.Now - startTime));

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
