using System;
using System.IO;

namespace TitanBotBaseTest.Helpers
{
    public static class FilesAndFolders
    {
        public const string TestFolderPath = @"..\TestFolder\";
        public static string TestFolderDir => Path.GetDirectoryName(TestFolderPath);
        public const string DataBasePath = TestFolderPath + @"Database\bot.db";


        public static bool DeleteTestFolder()
        {
            try
            {
                var fullPathDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, TestFolderDir));
                Directory.Delete(fullPathDir, true);
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed to delete TestFolder.");
                Console.Error.WriteLine(e);
                return false;
            }
        }
    }
}
