using DiscordBot;
using DiscordBot.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbLocation = @".\test.db";
            using (var bot = new BotClient(dbLocation))
            {
                bot.Database.QueryAsync(t => t.GetTable<Guild>().Insert(new Guild
                {
                    GuildId = 100,
                    TestText = "Hello :)"
                }));

                var saved = bot.Database.QueryAsync(t => t.GetTable<Guild>().Find(g => g.GuildId == 100)).Result;
            }
        }
    }
}
