using Discord;
using System.Collections.Generic;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Core.Services.Formatting
{
    public delegate string BeautifyDelegate<T>(T value);

    public interface IFormatterService
    {
        IReadOnlyDictionary<FormatType, IValueFormatter> Formatters { get; }

        void Register<T>(BeautifyDelegate<T> defaultFormatter, params (FormatType FormatType, BeautifyDelegate<T> Formatter)[] otherFormatters);

        IValueFormatter GetFormatter(FormatType format);

        IValueFormatter GetFormatter(IUser user);
    }
}