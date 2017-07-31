using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Formatting;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description(TitanBotResource.INFO_HELP_DESCRIPTION)]
    public class InfoCommand : Command
    {
        public static List<Action<EmbedBuilder, InfoCommand>> BuildActions { get; } = new List<Action<EmbedBuilder, InfoCommand>>();
        private ITextResourceManager TextManager { get; }


        static InfoCommand()
        {
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_GUILDS), c.Formatter.Beautify(c.Client.Guilds.Count)));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_CHANNELS), c.Formatter.Beautify(c.Client.Guilds.Sum(g => g.Channels.Count))));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_USERS), c.Formatter.Beautify(c.Client.Guilds.SelectMany(g => g.Users).Distinct().Count())));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_COMMANDS), c.Formatter.Beautify(c.CommandService.CommandList.Count)));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_CALLS), c.Formatter.Beautify(c.CommandService.CommandList.Sum(m => m.Calls.Count))));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_COMMANDS_USED), c.Formatter.Beautify(TotalCommands)));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_DATABASE_QUERIES), c.Formatter.Beautify(c.Database.TotalCalls)));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_RAM), $"{c.Formatter.Beautify((double)Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024))} / {c.Formatter.Beautify(PerformanceUtil.getAvailableRAM())}"));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_CPU), c.Formatter.Beautify(PerformanceUtil.getCurrentCPUUsage())));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_TIMERS), c.Formatter.Beautify(c.Scheduler.ActiveCount())));
            BuildActions.Add((b, c) => b.AddInlineField(c.TextResource.GetResource(TitanBotResource.INFO_FIELD_UPTIME), c.Formatter.Beautify(DateTime.Now - Process.GetCurrentProcess().StartTime)));
        }

        public InfoCommand(ITextResourceManager textManager)
        {
            TextManager = textManager;
        }

        [Call("Language")]
        [Usage(TitanBotResource.INFO_HELP_USAGE_LANGUAGE)]
        async Task ShowLanguageInfo()
        {
            var builder = new EmbedBuilder
            {
                Description = TextResource.GetResource(TitanBotResource.INFO_LANGUAGE_EMBED_DESCRIPTION)
            };
            foreach (var lang in TextManager.SupportedLanguages)
            {
                builder.AddInlineField(lang.ToString(), TextResource.Format(TitanBotResource.INFO_LANGUAGE_COVERAGE, Formatter.Beautify(TextManager.GetLanguageCoverage(lang)*100)));
            }

            await ReplyAsync(builder);
        }

        [Call("Technical")]
        [Usage(TitanBotResource.INFO_HELP_USAGE_TECHNICAL)]
        async Task ShowInfoAsync()
        {

            var builder = new EmbedBuilder
            {
                Title = TextResource.GetResource(TitanBotResource.INFO_TECHNICAL_TITLE, ReplyType.Info),
                Color = System.Drawing.Color.LawnGreen.ToDiscord(),
                Footer = new EmbedFooterBuilder
                {
                    Text = TextResource.Format(TitanBotResource.EMBED_FOOTER, BotUser.Username, "System Data")
                },
                Timestamp = DateTime.Now
            };

            foreach (var action in BuildActions)
            {
                action(builder, this);
            }

            await ReplyAsync(builder);
        }
    }
}
