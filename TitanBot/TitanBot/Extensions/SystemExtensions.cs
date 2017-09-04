using System.IO;

namespace System
{
    public static class SystemExtensions
    {
        public static Stream ToStream(this object text)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(text);
            sw.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}