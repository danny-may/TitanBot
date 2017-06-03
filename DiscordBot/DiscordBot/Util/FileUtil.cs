using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Util
{
    public static class FileUtil
    {
        public static void EnsureExists(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Path.Combine(AppContext.BaseDirectory, path);
            if (!File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                path = Path.GetDirectoryName(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
