using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.TextResource 
{
    class TextResourceCollection : ITextResourceCollection
    {
        private Dictionary<string, (string defaultText, string langText)> Values { get; }
        public double Coverage { get; }

        public string this[string key] => GetResource(key);

        public TextResourceCollection(double coverage, Dictionary<string, (string defaultText, string langText)> values)
        {
            Coverage = coverage;
            Values = values;
        }

        public string GetResource(string key)
        {
            if (!Values.ContainsKey(key))
                return key;
            var val = Values[key];
            return val.langText ?? val.defaultText ?? key;
        }

        public string Format(string key, params object[] items)
            => string.Format(GetResource(key), items);
    }
}
