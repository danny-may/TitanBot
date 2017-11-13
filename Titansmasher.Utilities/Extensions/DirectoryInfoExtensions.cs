using System;
using System.IO;

namespace Titansmasher.Extensions
{
    public static class DirectoryInfoExtensions
    {
        #region Publics

        public static void CleanDirectory(this DirectoryInfo directory, DateTime before = default(DateTime))
        {
            if (!directory.Exists)
                return;

            before = before == default(DateTime) ? DateTime.UtcNow.AddMonths(-2) : before;

            var backups = directory.GetFiles();

            foreach (var backup in backups)
                if (backup.LastWriteTimeUtc < before)
                    backup.Delete();
        }

        #endregion Publics
    }
}