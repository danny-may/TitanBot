using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using TitanBotBase;

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

        [TestMethod]
        public void SerializeTest()
        {
            var expected = typeof(BotClient);
            var serialized = "\"TitanBotBaseTest.BotClientUnitTests + QuickTest, DiscordBotTest, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null\"";
            var actual = JsonConvert.DeserializeObject<Type>(serialized);

            Assert.AreEqual(expected.FullName, actual.FullName);
        }
    }
}
