using Discord;
using Discord.WebSocket;
using System;
using TitanBot2.Common;
using TitanBot2.Services.Database;
using TitanBot2.Services.Database.Tables;

namespace TitanBot2.Services.Scheduler
{
    public class TimerContext
    {
        public Timer Timer { get; }
        public IUser User { get; }
        public SocketGuild Guild { get; }
        public ISocketMessageChannel Channel { get; }
        public DateTime EventTime { get; } = DateTime.Now;
        public BotDependencies Dependencies { get; }
        public BotClient BotClient { get { return Dependencies?.BotClient; } }
        public DiscordSocketClient Client { get { return Dependencies?.Client; } }
        public BotDatabase Database { get { return Dependencies?.Database; } }
        public TimerService TimerService { get { return Dependencies?.TimerService; } }
        public Logger Logger { get { return Dependencies?.Logger; } }

        public TimerContext(BotDependencies dependencies, Timer timer, DateTime execTime)
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
