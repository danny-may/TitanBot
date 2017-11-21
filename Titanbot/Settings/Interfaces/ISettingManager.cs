using Discord;

namespace Titanbot.Settings.Interfaces
{
    public interface ISettingManager
    {
        ISettingGroup Global { get; }

        IGroupCollection this[ulong id] { get; }
        IGroupCollection this[IEntity<ulong> id] { get; }
        IGroupCollection GroupsFor(ulong id);
        IGroupCollection GroupsFor(IEntity<ulong> id);
    }
}