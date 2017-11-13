using System;
using System.Collections.Generic;
using System.IO;

namespace Titansmasher.Utilities.Extensions
{
    public static class FileInfoExtensions
    {
        #region Publics

        #region Path Modifiers

        public static FileInfo WithTimestamp(this FileInfo file, DateTime timestamp = default(DateTime), string format = "_ddMMyyyy_hhmmss")
            => file.ModifyFileName(s => s + string.Format($"{{0:{format}}}", timestamp == default(DateTime) ? DateTime.UtcNow : timestamp));

        public static FileInfo ModifyFileName(this FileInfo file, Func<string, string> editor)
        {
            var directory = file.DirectoryName;
            var filename = editor(file.GetFileNameWithoutExtension());
            var extension = file.Extension;

            return new FileInfo(Path.Combine(directory, filename + extension));
        }

        public static FileInfo ModifyDirectory(this FileInfo file, Func<string, string> editor)
        {
            var directory = editor(file.Directory.FullName);
            var fileName = file.Name;

            return new FileInfo(Path.Combine(directory, fileName));
        }

        #endregion Path Modifiers

        #region EnsureDirectory

        public static FileInfo EnsureDirectory(this FileInfo file)
        {
            if (!file.Directory.Exists)
                file.Directory.Create();
            return file;
        }

        #endregion EnsureDirectory

        #region EnsureExists

        public static FileInfo EnsureExists(this FileInfo file, Func<byte[]> content)
            => EnsureExists(file, content, data => File.WriteAllBytes(file.FullName, data));

        public static FileInfo EnsureExists(this FileInfo file, byte[] content)
            => EnsureExists(file, () => content);

        public static FileInfo EnsureExists(this FileInfo file, Func<string> content)
            => EnsureExists(file, content, data => File.WriteAllText(file.FullName, data));

        public static FileInfo EnsureExists(this FileInfo file, string content)
            => EnsureExists(file, () => content);

        public static FileInfo EnsureExists(this FileInfo file)
            => EnsureExists(file, "");

        #endregion EnsureExists

        #region Read

        public static int Read(this FileInfo file, byte[] buffer, int offset, int length)
        {
            using (var fs = file.OpenRead())
                return fs.Read(buffer, offset, length);
        }

        public static byte[] ReadAllBytes(this FileInfo file)
            => File.ReadAllBytes(file.FullName);

        public static string[] ReadLines(this FileInfo file)
            => File.ReadAllLines(file.FullName);

        public static string ReadAllText(this FileInfo file)
            => File.ReadAllText(file.FullName);

        #endregion Read

        #region Write

        public static void WriteAllBytes(this FileInfo file, byte[] data)
            => File.WriteAllBytes(file.FullName, data);

        public static void WriteAllLines(this FileInfo file, string[] lines)
            => File.WriteAllLines(file.FullName, lines);

        public static void WriteAllText(this FileInfo file, string text)
            => File.WriteAllText(file.FullName, text);

        #endregion Write

        #region Append

        public static void AppendAllText(this FileInfo file, string text)
            => File.AppendAllText(file.FullName, text);

        public static void AppendAllLines(this FileInfo file, IEnumerable<string> lines)
            => File.AppendAllLines(file.FullName, lines);

        #endregion Append

        #region Misc

        public static string GetFileNameWithoutExtension(this FileInfo file)
            => Path.GetFileNameWithoutExtension(file.FullName);

        #endregion Misc

        #endregion Publics

        #region Privates

        private static FileInfo EnsureExists<TData>(FileInfo file, Func<TData> content, Action<TData> writer)
        {
            file.EnsureDirectory();
            if (!file.Exists)
                writer(content());

            return file;
        }

        #endregion Privates
    }
}