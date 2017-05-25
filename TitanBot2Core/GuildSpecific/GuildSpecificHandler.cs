using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.GuildSpecific
{
    public abstract class GuildSpecificHandler
    {
        protected BotDependencies Dependencies;

        public GuildSpecificHandler(BotDependencies dependencies)
        {
            Dependencies = dependencies;
        }

        internal virtual Task HandleBanned(SocketUser user, SocketGuild guild)
            => Task.CompletedTask;

        internal virtual Task HandleUnban(SocketUser user, SocketGuild guild)
            => Task.CompletedTask;

        internal virtual Task HandleJoin(SocketGuildUser user)
            => Task.CompletedTask;

        internal virtual Task HandleLeave(SocketGuildUser user)
            => Task.CompletedTask;

        internal virtual Task HandleGUpdate(SocketGuildUser oldUser, SocketGuildUser newUser)
            => Task.CompletedTask;
    }
}
