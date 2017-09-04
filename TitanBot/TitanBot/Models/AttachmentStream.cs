using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TitanBot.Core.Services.Messaging;

namespace TitanBot.Models
{
    public class AttachmentStream : IAttachmentStream
    {
        public Stream FileStream { get; }
        public string FileName { get; }

        private AttachmentStream(Stream stream, string name)
        {
            FileStream = stream;
            FileName = name;
        }

        public static AttachmentStream From(Stream stream, string fileName)
            => new AttachmentStream(stream, fileName);

        public static AttachmentStream From(string text, string fileName)
            => new AttachmentStream(text.ToStream(), fileName);

        public static AttachmentStream FromFile(string filepath, string fileName = null)
        {
            var info = new FileInfo(filepath);
            if (!info.Exists)
                throw new FileNotFoundException("", filepath);
            return new AttachmentStream(info.OpenRead(), fileName ?? info.Name);
        }
    }
}