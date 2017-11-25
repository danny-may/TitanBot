using Titansmasher.Services.Display;

namespace Titanbot.Settings.DefaultSettings
{
    public class GeneralUserSettings
    {
        public string Prefix { get; set; }
        public Language Language { get; set; } = Language.Default;
        public Format Format { get; set; } = Format.Default;
        public bool UseEmbeds { get; set; } = true;
    }
}