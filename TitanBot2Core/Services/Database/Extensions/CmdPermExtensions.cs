using System;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Models;

namespace TitanBot2.Services.Database.Extensions
{
    public class CmdPermExtensions : DatabaseExtension<CmdPerm>
    {
        public CmdPermExtensions(TitanbotDatabase db) : base(db) { }

        public async Task<CmdPerm> GetCmdPerm(ulong guildid, string command)
            => await GetCmdPerm(guildid, command, null);
        public async Task<CmdPerm> GetCmdPerm(ulong guildid, string command, Func<Exception, Task> handler)
            => await Database.QueryAsync(conn => conn.CmdPermTable.FindOne(c => c.guildId == guildid && c.commandname == command), handler);

        public async Task SetCmdPerm(ulong guildid, string command, string subCommand, ulong[] roleIds = null, ulong? permission = null)
            => await SetCmdPerm(guildid, command, subCommand, null, roleIds, permission);
        public async Task SetCmdPerm(ulong guildid, string command, string subCommand, Func<Exception, Task> handler, ulong[] roleIds = null, ulong? permission = null)
        {
            var newPerm = new CmdPerm
            {
                guildId = guildid,
                commandname = command,
                permissionId = permission,
                roleIds = roleIds
            };

            var current = await GetCmdPerm(guildid, command);

            await Database.QueryAsync(conn =>
            {
                if (current != null)
                    conn.CmdPermTable.Delete(current.entryId);
                if (roleIds != null || permission != null)
                    conn.CmdPermTable.Insert(newPerm);
            }, handler);
        }

        public async Task ResetCmdPerm(ulong guildid, string command)
            => await ResetCmdPerm(guildid, command, null);
        public async Task ResetCmdPerm(ulong guildid, string command, Func<Exception, Task> handler)
            => await Database.QueryAsync(conn => conn.CmdPermTable.Delete(c => c.guildId == guildid && c.commandname == command));
    }
}
