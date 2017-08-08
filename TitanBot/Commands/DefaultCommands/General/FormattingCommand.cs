using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Formatting;
using TitanBot.Replying;
using TitanBot.Settings;
using static TitanBot.TBLocalisation.Commands;
using static TitanBot.TBLocalisation.Help;

namespace TitanBot.Commands.DefaultCommands.General
{
    [Description(Desc.FORMATTING)]
    [Alias("Format", "Output")]
    class FormattingCommand : Command
    {
        [Call]
        [Usage(Usage.FORMATTING)]
        [Alias("List")]
        async Task ListFormatsAsync()
        {
            var builder = new LocalisedEmbedBuilder
            {
                Color = System.Drawing.Color.Chartreuse.ToDiscord()
            }.WithTitle(FormattingText.LIST_TITLE);
            foreach (var format in Formatter.KnownFormats)
                builder.AddField(f => f.WithName(format.GetName()).WithValue(format.GetDescription()));

            await ReplyAsync(builder);
        }

        [Call("Use")]
        [Usage(Usage.FORMATTING_USE)]
        [Alias("Set")]
        async Task SetFormatAsync(FormatType format)
        {
            UserSettings.Edit<GeneralUserSetting>(s => s.FormatType = format);
            await ReplyAsync(FormattingText.SET_SUCCESS, ReplyType.Success, format);
        }
    }
}
