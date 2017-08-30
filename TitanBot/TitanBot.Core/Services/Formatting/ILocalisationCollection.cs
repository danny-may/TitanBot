using System;
using System.Collections.Generic;
using System.Text;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Core.Services.Formatting
{
    public interface ILocalisationCollection
    {
        string this[string index] { get; }
        double Coverage { get; }
        IValueFormatter Formatter { get; }
        FormatType FormatType { get; }

        string Format(string key, params object[] items);

        string GetResource(string key);
    }
}