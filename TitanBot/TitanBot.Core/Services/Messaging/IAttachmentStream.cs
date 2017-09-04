using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TitanBot.Core.Services.Messaging
{
    public interface IAttachmentStream
    {
        Stream FileStream { get; }
        string FileName { get; }
    }
}