using System;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Core.Services.Formatting
{
    public interface IValueFormatter
    {
        FormatType FormatType { get; }

        Type[] KnownTypes { get; }

        string Beautify<T>(T value);

        string Beautify(object value);

        string Beautify(Type type, object value);
    }
}