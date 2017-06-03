using TitanBotBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBaseTest
{
    [TestClass]
    public class BotClientUnitTests
    {
        [TestMethod]
        public void BasicConstructAndStart()
        {
            using (var bot = new BotClient())
            {
                Assert.IsNotNull(bot.Database);
                Assert.IsNotNull(bot.DiscordClient);
            }

        }
    }
}
