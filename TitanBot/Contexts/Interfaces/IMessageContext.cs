using Discord;
using Discord.WebSocket;
using TitanBot.Formatting;
using TitanBot.Replying;
using TitanBot.Settings;

namespace TitanBot.Contexts
{
    public interface IMessageContext
    {
        IUser Author { get; }
        IReplier Replier { get; }
        ITextResourceCollection TextResource { get; }
        ITextResourceManager TextManager { get; }
        ValueFormatter Formatter { get; }
        IMessageChannel Channel { get; }
        DiscordSocketClient Client { get; }
        IGuild Guild { get; }
        IUserMessage Message { get; }

        ISettingContext GlobalSettings { get; }
        ISettingContext ChannelSettings { get; }
        ISettingContext UserSettings { get; }
        ISettingContext GuildSettings { get; }

        GeneralGlobalSetting GeneralGlobalSetting { get; }
        GeneralGuildSetting GeneralGuildSetting { get; }
        GeneralUserSetting GeneralUserSetting { get; }
    }
}
