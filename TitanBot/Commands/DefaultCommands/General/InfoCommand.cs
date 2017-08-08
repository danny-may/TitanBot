using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Dependencies;
using TitanBot.Formatting;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description(Desc.INFO)]
    public class InfoCommand : Command
    {
        public static List<Func<InfoCommand, (ILocalisable<string> Title, object Value, bool IsInline)>> TechnicalActions { get; } = new List<Func<InfoCommand, (ILocalisable<string>, object, bool)>>();
        private ITextResourceManager TextManager { get; }
        private IDependencyFactory Factory { get; }

        static InfoCommand()
        {
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_GUILDS, c.Client.Guilds.Count, true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_CHANNELS, c.Client.Guilds.Sum(g => g.Channels.Count), true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_USERS, c.Client.Guilds.SelectMany(g => g.Users).Distinct().Count(), true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_COMMANDS, c.CommandService.CommandList.Count, true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_CALLS, c.CommandService.CommandList.Sum(m => m.Calls.Count), true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_COMMANDS_USED, TotalCommands, true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_DATABASE_QUERIES, c.Database.TotalCalls, true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_FACTORY_BUILT, c.Factory.History.Count, true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_RAM, c.Beautify((double)Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024)) + " / " + c.Beautify(PerformanceUtil.getAvailableRAM()), true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_CPU, PerformanceUtil.getCurrentCPUUsage(), true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_TIMERS, c.Scheduler.ActiveCount(), true));
            TechnicalActions.Add(c => ((LocalisedString)InfoText.FIELD_UPTIME, DateTime.Now - Process.GetCurrentProcess().StartTime, true));
        }

        public InfoCommand(ITextResourceManager textManager, IDependencyFactory factory)
        {
            TextManager = textManager;
            Factory = factory;
        }

        [Call]
        [Usage(Usage.INFO)]
        async Task ShowInfoAsync()
        {
            var builder = new LocalisedEmbedBuilder
            {
                Title = new LocalisedString(InfoText.TITLE, ReplyType.Info),
                Color = System.Drawing.Color.LawnGreen.ToDiscord(),
                Footer = new LocalisedFooterBuilder().WithText(InfoText.FOOTER, BotUser.Username),
                Timestamp = DateTime.Now
            };

            foreach (var action in TechnicalActions)
            {
                var settings = action(this);
                builder.AddInlineField(f => f.WithName(settings.Title).WithValue(settings.Value));
            }

            await ReplyAsync(builder);
        }
    }
}
