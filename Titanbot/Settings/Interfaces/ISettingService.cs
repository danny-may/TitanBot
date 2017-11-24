using Discord;

namespace Titanbot.Settings.Interfaces
{
    public interface ISettingService
    {
        ISettingCollection Global { get; }

        ISettingCollection this[decimal id] { get; }
        ISettingCollection this[IEntity<decimal> id] { get; }
        ISettingCollection CollectionFor(decimal id);
        ISettingCollection CollectionFor(IEntity<decimal> id);
    }
}