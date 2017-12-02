namespace Titanbot.Permissions.Interfaces
{
    public interface IPermissionManager
    {
        bool IsBlacklist(ulong guildId, string key);
        ulong[] GetListedChannels(ulong guildId, string key);
        ulong[] GetRoles(ulong guildId, string key);
        ulong GetPermission(ulong guildid, string key);

        void Blacklist(ulong guildId, string key);
        void Blacklist(ulong guildId, string key, ulong channelId);
        void Whitelist(ulong guildId, string key);
        void Whitelist(ulong guildId, string key, ulong channelId);

        void SetRoles(ulong guildId, string key, ulong[] roleIds);
        void AddRoles(ulong guildId, string key, ulong[] roleIds);
        void SetPermissions(ulong guildId, string key, ulong permissions);
    }
}