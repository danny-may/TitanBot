using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Formatting;
using TitanBot.Replying;
using TitanBot.Settings;
using TitanBot.Util;
using static TitanBot.TBLocalisation.Commands;
using static TitanBot.TBLocalisation.Help;

namespace TitanBot.Commands.DefaultCommands.General
{
    [Description(Desc.LANGUAGE)]
    [Alias("Language", "Locale")]
    class LanguagesCommand : Command
    {
        ITextResourceManager TextManager { get; }
        public LanguagesCommand(ITextResourceManager textManager)
        {
            TextManager = textManager;
        }

        [Call("Export")]
        [Usage(Usage.LANGUAGE_EXPORT)]
        [RequireOwner]
        async Task ExportAsync(Locale language)
        {
            var exported = TextManager.Export(language);
            await Reply().WithMessage(new LocalisedString(LanguageText.EXPORTED, ReplyType.Success, language))
                         .WithAttachment(() => exported.ToStream(), $"{language}.json")
                         .SendAsync();
        }

        [Call("Import")]
        [Usage(Usage.LANGUAGE_IMPORT)]
        [RequireOwner]
        async Task ImportAsync(string language)
        {
            Locale lang = language;
            if (Message.Attachments.Count != 1)
            {
                await ReplyAsync(LanguageText.ATTACHMENT_MISSING, ReplyType.Error);
                return;
            }

            var attachment = await Downloader.GetString(new Uri(Message.Attachments.First().Url));
            if (string.IsNullOrWhiteSpace(attachment))
            {
                await ReplyAsync(LanguageText.ATTACHMENT_EMPTY, ReplyType.Error);
                return;
            }

            TextManager.Import(lang, attachment);
            await ReplyAsync(LanguageText.IMPORTED, ReplyType.Success, lang);
        }

        [Call("Reload")]
        [Usage(Usage.LANGUAGE_RELOAD)]
        [RequireOwner]
        async Task ReloadAsync()
        {
            TextManager.Reload();
            await ReplyAsync(LanguageText.RELOADED, ReplyType.Success);
        }

        [Call("Use")]
        [Alias("Set")]
        [Usage(Usage.LANGUAGE_USE)]
        async Task UseLanguageAsync(Locale language)
        {
            UserSettings.Edit<GeneralUserSetting>(s => s.Language = language);
            await ReplyAsync(LanguageText.CHANGED, ReplyType.Success, language);
        }

        [Call]
        [Usage(Usage.LANGUAGE)]
        async Task ListLanguagesAsync()
        {
            var builder = new LocalisedEmbedBuilder
            {
                Color = System.Drawing.Color.DarkKhaki.ToDiscord()
            }.WithDescription(LanguageText.EMBED_DESCRIPTION);
            foreach (var lang in TextManager.SupportedLanguages)
            {
                builder.AddField(f => f.WithRawName(lang.ToString()).WithValue(LanguageText.COVERAGE, TextManager.GetLanguageCoverage(lang) * 100));
            }

            await ReplyAsync(builder);
        }
    }
}
