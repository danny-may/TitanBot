using System;
using Titanbot.Permissions.Interfaces;

namespace Titanbot.Permissions
{
    public class PermissionManager : IPermissionManager
    {
        public void AddRoles(ulong guildId, string key, ulong[] roleIds)
        {
            throw new NotImplementedException();
        }

        public void Blacklist(ulong guildId, string key)
        {
            throw new NotImplementedException();
        }

        public void Blacklist(ulong guildId, string key, ulong channelId)
        {
            throw new NotImplementedException();
        }

        public ulong[] GetListedChannels(ulong guildId, string key)
        {
            throw new NotImplementedException();
        }

        public ulong GetPermission(ulong guildid, string key)
        {
            throw new NotImplementedException();
        }

        public ulong[] GetRoles(ulong guildId, string key)
        {
            throw new NotImplementedException();
        }

        public bool IsBlacklist(ulong guildId, string key)
        {
            throw new NotImplementedException();
        }

        public void SetPermissions(ulong guildId, string key, ulong permissions)
        {
            throw new NotImplementedException();
        }

        public void SetRoles(ulong guildId, string key, ulong[] roleIds)
        {
            throw new NotImplementedException();
        }

        public void Whitelist(ulong guildId, string key)
        {
            throw new NotImplementedException();
        }

        public void Whitelist(ulong guildId, string key, ulong channelId)
        {
            throw new NotImplementedException();
        }
    }
}