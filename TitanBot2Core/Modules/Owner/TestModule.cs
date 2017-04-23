using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Models;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Modules.Owner
{
    public partial class OwnerModule
    {
        [Group("Test")]
        [Summary("Test command, could be anything, we just dont know :o")]
        public class TestModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            [Remarks("Does something probably")]
            public async Task TestAsync()
            {
                await Context.Database.QueryAsync(conn =>
                {
                    conn.GetCollection<Timer>().Insert(new Timer
                    {
                        GuildId = Context.Guild?.Id ?? Context.Channel.Id,
                        UserId = Context.User.Id,
                        ChannelId = Context.Channel.Id,
                        MessageId = Context.Message.Id,
                        Callback = EventCallback.TitanLordNow,
                        To = DateTime.Now,
                        From = DateTime.Now,
                        SecondInterval = 1
                    });
                });

                var userTimers = await Context.Database.QueryAsync(conn =>
                conn.GetCollection<Timer>().Find(t => t.UserId == Context.User.Id));
            }
        }
    }
}
