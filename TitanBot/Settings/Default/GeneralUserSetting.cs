using TitanBot.Formatting;

namespace TitanBot.Settings
{
    public class GeneralUserSetting
    {
        public int FormatType { get; set; } = FormattingType.DEFAULT;
        public string Language { get; set; } = Locale.DEFAULT;
        public bool UseEmbeds { get; set; } = true;
    }
}
