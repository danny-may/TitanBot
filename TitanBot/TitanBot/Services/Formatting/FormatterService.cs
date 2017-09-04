using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Services.Formatting
{
    public class FormatterService : IFormatterService
    {
        #region Fields

        private readonly ConcurrentDictionary<FormatType, ValueFormatter> _formatters = new ConcurrentDictionary<FormatType, ValueFormatter>();

        #endregion Fields

        #region Constructors

        public FormatterService()
        {
        }

        #endregion Constructors

        #region IFormatterService

        public IReadOnlyDictionary<FormatType, IValueFormatter> Formatters => _formatters.ToImmutableDictionary(k => k.Key, v => v.Value as IValueFormatter);

        public IValueFormatter GetFormatter(FormatType format)
            => _formatters.TryGetValue(format, out var formatter) ? formatter : null;

        public IValueFormatter GetFormatter(IUser user)
        {
            throw new NotImplementedException();
        }

        public void Register<T>(BeautifyDelegate<T> defaultFormatter, params (FormatType FormatType, BeautifyDelegate<T> Formatter)[] otherFormatters)
        {
            var formatters = new(FormatType FormatType, BeautifyDelegate<T> Formatter)[] { (FormatType.DEFAULT, defaultFormatter) }.Concat(otherFormatters);
            if (formatters.Select(f => f.FormatType).Distinct().Count() != formatters.Count())
                throw new ArgumentException("More than one formatter was specified for a given FormatType", nameof(otherFormatters));

            foreach (var f in formatters)
                _formatters.GetOrAdd(f.FormatType, k => new ValueFormatter(k)).AddFormatter(f.Formatter);
        }

        #endregion IFormatterService
    }
}