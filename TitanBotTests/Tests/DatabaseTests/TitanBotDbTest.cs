using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitanBot.Database;
using TitanBot.Logger;
using TitanBotBaseTest.Helpers;
using TitanBotBaseTest.MockModels;

namespace TitanBotBaseTest.Tests.DatabaseTests
{
    [TestClass]
    public class TitanBotDbTest
    {
        [TestMethod]
        public void CreateDatabase()
        {
            const string path = FilesAndFolders.DataBasePath;
            var mockLogger = new MockLogger();

            var db = new TitanBotDb(path, mockLogger);
            Console.WriteLine();

            Assert.IsNotNull(db);

            // Cleanup
            Assert.IsTrue(FilesAndFolders.DeleteTestFolder());
        }
    }
}