using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.CommandService.Flags
{
    public struct FlagInfo
    {
        public Type FlagType { get; }
        public string ShortKey { get; }
        public string LongKey { get; }
        public string Description { get; }

        public FlagInfo(Type flagType, string shortKey, string longKey, string description)
        {
            FlagType = flagType;
            ShortKey = shortKey;
            LongKey = longKey;
            Description = description;
        }

        public override string ToString()
            => $"`--{LongKey}`/`-{ShortKey}` = {Description}";
    }
}
