using Titansmasher.Services.Display;

namespace Titanbot.Settings.DefaultSettings
{
    public class GeneralGuildSettings
    {
        public string Prefix { get; set; }
        public ulong? PermissionOverride { get; set; } = 8;
        public ulong? RoleOverride { get; set; }
        public string DateFormat { get; set; } = "dd/MM/yy";
        public string TimeFormat { get; set; } = "HH:mm:ss";
        public Language DefaultLanguage { get; set; } = Language.Default;
    }
}