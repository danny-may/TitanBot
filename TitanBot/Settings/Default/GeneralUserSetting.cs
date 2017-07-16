using TitanBot.Formatting;

namespace TitanBot.Settings
{
    public class GeneralUserSetting
    {
        public uint FormatType { get; set; } = FormattingType.DEFAULT;
        public string Language { get; set; } = Locale.DEFAULT;
        public bool UseEmbeds { get; set; } = true;
    }
}
