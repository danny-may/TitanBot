using System;
using TitanBot.Core.Models.Contexts;

namespace TitanBot.Core.Services.TypeReader
{
    public interface ITypeReaderResultCache : IDisposable
    {
        ITypeReaderResultCache BeginCache();

        ITypeReaderResult Read<T>(IMessageContext context, string text);
        ITypeReaderResult Read(IMessageContext context, string text, Type type);
    }
}