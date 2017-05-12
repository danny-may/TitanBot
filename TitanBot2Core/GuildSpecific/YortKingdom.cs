using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using TitanBot2.Common;

namespace TitanBot2.GuildSpecific
{
    public class YortKingdom : GuildSpecificHandler
    {
        public YortKingdom(TitanbotDependencies dependencies) : base(dependencies)
        {
        }

        internal override async Task HandleJoin(SocketGuildUser user)
        {
            var serverOwners = Dependencies.Client.Guilds.Where(g => g.MemberCount > 20).Select(g => g.Owner.Id);
            var ownerRole = user.Guild.GetRole(312494566500728844);
            if (serverOwners.Contains(user.Id))
                await user.AddRoleAsync(ownerRole);
        }
    }
}
