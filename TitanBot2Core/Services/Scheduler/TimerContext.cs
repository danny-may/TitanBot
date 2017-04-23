using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.Database;
using TitanBot2.Services.Database.Models;

namespace TitanBot2.Services.Scheduler
{
    public class TimerContext
    {
        public Timer Timer { get; }
        public IUser User { get; }
        public SocketGuild Guild { get; }
        public ISocketMessageChannel Channel { get; }
        public DateTime EventTime { get; } = DateTime.Now;
        public TitanbotDependencies Dependencies { get; }
        public TitanBot TitanBot { get { return Dependencies?.TitanBot; } }
        public DiscordSocketClient Client { get { return Dependencies?.Client; } }
        public TitanbotDatabase Database { get { return Dependencies?.Database; } }
        public TimerService TimerService { get { return Dependencies?.TimerService; } }
        public Logger Logger { get { return Dependencies?.Logger; } }

        public TimerContext(TitanbotDependencies dependencies, Timer timer, DateTime execTime)
        {
            EventTime = execTime;
            Dependencies = dependencies;
            Timer = timer;
            User = Client.GetUser(timer.UserId);
            if (timer.GuildId != null)
                Guild = Client.GetGuild(timer.GuildId.Value);
            Channel = Client.GetChannel(timer.ChannelId) as ISocketMessageChannel;
        }
    }
}
