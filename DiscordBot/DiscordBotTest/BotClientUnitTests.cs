using DiscordBot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTest
{
    [TestClass]
    public class BotClientUnitTests
    {
        [TestMethod]
        public void BasicConstructAndStart()
        {
            using (var bot = new BotClient(@".\database.db"))
            {
                Assert.IsNotNull(bot.Database);
                Assert.IsNotNull(bot.DiscordClient);
            }

        }
    }
}
