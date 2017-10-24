namespace System.IO
{
    public static class FileExtensions
    {
        public static void EnsureFile(string file)
            => EnsureFile(new FileInfo(file));

        public static void EnsureFile(FileInfo file)
        {
            EnsureDirectory(file.Directory);
            if (!file.Exists)
                file.Create().Dispose();
        }

        public static void EnsureDirectory(string directory)
            => EnsureDirectory(new DirectoryInfo(directory));

        public static void EnsureDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
                directory.Create();
        }
    }
}