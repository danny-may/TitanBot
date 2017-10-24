using Discord;

namespace TitanBot.Core.Services.Setting
{
    public interface ISettingService
    {
        ISettingCollection GetGlobalContext();

        ISettingCollection GetContext(ulong id);
        ISettingCollection GetContext(IEntity<ulong> entity);

        void DestroyContext(ulong id);
        void DestroyContext(IEntity<ulong> entity);
        void DestroyContext(ISettingCollection context);
    }
}