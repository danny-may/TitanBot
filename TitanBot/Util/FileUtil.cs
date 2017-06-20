using System;
using System.IO;

namespace TitanBot.Util
{
    public static class FileUtil
    {
        public static void EnsureDirectory(string path)
        {
            path = Path.GetDirectoryName(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string GetAbsolutePath(string path)
        {
            if (!Path.IsPathRooted(path))
                return Path.Combine(AppContext.BaseDirectory, path);
            return path;
        }

        public static string GetTimestamp(DateTime time = default(DateTime))
        {
            if (time == default(DateTime))
                time = DateTime.Now;

            return $"{time.Day}{time.Month}{time.Year}_{time.Hour}{time.Minute}{time.Second}";
        }
    }
}
