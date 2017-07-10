using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.TextResource;
using TitanBot.Util;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description("Displays some technical information about me")]
    public class InfoCommand : Command
    {
        public static List<Action<EmbedBuilder, InfoCommand>> BuildActions { get; } = new List<Action<EmbedBuilder, InfoCommand>>();
        private ITextResourceManager TextManager { get; }


        static InfoCommand()
        {
            BuildActions.Add((b, c) => b.AddInlineField("Guilds", c.Formatter.Beautify(c.Client.Guilds.Count)));
            BuildActions.Add((b, c) => b.AddInlineField("Channels", c.Formatter.Beautify(c.Client.Guilds.Sum(g => g.Channels.Count))));
            BuildActions.Add((b, c) => b.AddInlineField("Users", c.Formatter.Beautify(c.Client.Guilds.SelectMany(g => g.Users).Distinct().Count())));
            BuildActions.Add((b, c) => b.AddInlineField("Loaded commands", c.Formatter.Beautify(c.CommandService.CommandList.Count)));
            BuildActions.Add((b, c) => b.AddInlineField("Loaded calls", c.Formatter.Beautify(c.CommandService.CommandList.Sum(m => m.Calls.Count))));
            BuildActions.Add((b, c) => b.AddInlineField("Commands used this session", c.Formatter.Beautify(TotalCommands)));
            BuildActions.Add((b, c) => b.AddInlineField("Database queries", c.Formatter.Beautify(c.Database.TotalCalls)));
            BuildActions.Add((b, c) => b.AddInlineField("RAM", $"{c.Formatter.Beautify((double)Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024))} / {c.Formatter.Beautify(PerformanceUtil.getAvailableRAM())}"));
            BuildActions.Add((b, c) => b.AddInlineField("CPU", c.Formatter.Beautify(PerformanceUtil.getCurrentCPUUsage())));
            BuildActions.Add((b, c) => b.AddInlineField("Active Timers", c.Formatter.Beautify(c.Scheduler.ActiveCount())));
            BuildActions.Add((b, c) => b.AddInlineField("Uptime", c.Formatter.Beautify(DateTime.Now - Process.GetCurrentProcess().StartTime)));
        }

        public InfoCommand(ITextResourceManager textManager)
        {
            TextManager = textManager;
        }

        [Call("Language")]
        [Usage("Displays info about what languages are supported")]
        async Task ShowLanguageInfo()
        {
            var builder = new EmbedBuilder
            {
                Description = TextResource.GetResource("INFO_LANGUAGE_EMBED_DESCRIPTION")
            };
            foreach (var lang in TextManager.SupportedLanguages)
            {
                builder.AddInlineField(lang.ToString(), Formatter.Beautify(TextManager.GetLanguageCoverage(lang)*100) + "% Coverage");
            }

            await ReplyAsync("", embed: builder.Build());
        }

        [Call]
        [Usage("Displays the info")]
        async Task ShowInfoAsync()
        {

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

            foreach (var action in BuildActions)
            {
                action(builder, this);
            }

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
