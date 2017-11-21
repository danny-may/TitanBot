namespace Titanbot.Settings.Interfaces
{
    public interface IGroupCollection
    {
        ulong Id { get; }

        ISettingGroup DefaultGroup { get; }

        ISettingGroup this[string key] { get; }

        string[] AvailableGroups { get; }
        ISettingGroup CreateGroup(string name);
        ISettingGroup GetGroup(string name);
        void DeleteGroup(string name);
        void ClearGroups();
    }
}