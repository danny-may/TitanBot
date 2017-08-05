using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot
{
    public static partial class TBLocalisation
    {
        public static class Types
        {
            public static string TIMESPAN = typeof(TimeSpan).Name;
            public static string STRING = typeof(string).Name;
            public static string LONG = typeof(long).Name;
            public static string INT = typeof(int).Name;
            public static string SHORT = typeof(short).Name;
            public static string ULONG = typeof(ulong).Name;
            public static string UINT = typeof(uint).Name;
            public static string USHORT = typeof(ushort).Name;
            public static string DOUBLE = typeof(double).Name;
            public static string FLOAT = typeof(float).Name;
            public static string DECIMAL = typeof(decimal).Name;
            public static string BOOLEAN = typeof(bool).Name;
            public static string DATETIME = typeof(DateTime).Name;

            public static IReadOnlyDictionary<string, string> Defaults { get; }
                = new Dictionary<string, string>
                {
                    { typeof(TimeSpan).Name, "a timespan" },
                    { typeof(string).Name, "some text" },
                    { typeof(long).Name, "an integer" },
                    { typeof(int).Name, "an integer" },
                    { typeof(short).Name, "an integer" },
                    { typeof(ulong).Name, "a positive integer" },
                    { typeof(uint).Name, "a positive integer" },
                    { typeof(ushort).Name, "a positive integer" },
                    { typeof(double).Name, "a number" },
                    { typeof(float).Name, "a number" },
                    { typeof(decimal).Name, "a number" },
                    { typeof(bool).Name, "true/false" },
                    { typeof(DateTime).Name, "a date/time" }
                }.ToImmutableDictionary();
        }
    }
}
