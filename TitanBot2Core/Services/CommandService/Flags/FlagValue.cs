using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.CommandService.Flags
{
    public struct FlagValue
    {
        public string Key { get; }
        public object Value { get; }

        public FlagValue(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return $"-{Key} {Value}";
        }
    }
}
