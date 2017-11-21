namespace Titanbot.Settings.Interfaces
{
    public interface ISettingGroup
    {
        ulong Id { get; }
        string GroupName { get; }

        TSetting Get<TSetting>();
        void Update<TSetting>(TSetting setting);
        void Reset<TSetting>();
        void Clear();
    }
}