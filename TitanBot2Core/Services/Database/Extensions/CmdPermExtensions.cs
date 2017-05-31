using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Tables;

namespace TitanBot2.Services.Database.Extensions
{
    public class CmdPermExtensions : DatabaseExtension<CmdPerm>
    {
        public CmdPermExtensions(BotDatabase db) : base(db) { }

        public async Task<CmdPerm> GetCmdPerm(ulong guildid, string command)
            => await GetCmdPerm(guildid, command, null);
        public async Task<CmdPerm> GetCmdPerm(ulong guildid, string command, Func<Exception, Task> handler)
            => await Database.QueryAsync(conn => conn.CmdPermTable.FindOne(c => c.guildId == guildid && c.commandname == command.ToLower()), handler);

        public async Task SetCmdPerm(ulong guildid, string command, ulong[] roleIds = null, ulong? permission = null)
            => await SetCmdPerm(guildid, command, null, roleIds, permission);
        public async Task SetCmdPerm(ulong guildid, string command, Func<Exception, Task> handler, ulong[] roleIds = null, ulong? permission = null)
        {
            command = command.ToLower();
            var existing = await GetCmdPerm(guildid, command) ?? new CmdPerm
            {
                commandname = command,
                guildId = guildid
            };

            existing.roleIds = roleIds;
            existing.permissionId = permission;

            await Database.QueryAsync(conn =>
            {
                if (roleIds != null || permission != null)
                    conn.CmdPermTable.Upsert(existing);
            }, handler);
        }

        public async Task BlackList(ulong guildid, string command, ulong[] channels, bool blacklist)
        {
            command = command.ToLower();
            var existing = await GetCmdPerm(guildid, command) ?? new CmdPerm
            {
                commandname = command,
                guildId = guildid
            };
            if (blacklist)
                existing.blackListed = channels.Concat(existing.blackListed ?? new ulong[0]).Distinct().ToArray();
            else
                existing.blackListed = (existing.blackListed ?? new ulong[0]).Except(channels).Distinct().ToArray();
            await Database.QueryAsync(conn => conn.CmdPermTable.Upsert(existing));
        }

        public async Task ResetCmdPerm(ulong guildid, string command)
            => await ResetCmdPerm(guildid, command, null);
        public async Task ResetCmdPerm(ulong guildid, string command, Func<Exception, Task> handler)
            => await Database.QueryAsync(conn => conn.CmdPermTable.Delete(c => c.guildId == guildid && c.commandname == command));
    }
}
