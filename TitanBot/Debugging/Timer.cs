using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot.Debugging
{
    public class DebugTimer
    {
        public DateTime Start { get; } = DateTime.Now;
        public DateTime[] Marks => _marks.ToArray();
        public TimeSpan[] Periods => _marks.ToArray().Pair().Select(p => p.Second - p.First).ToArray();

        private List<DateTime> _marks = new List<DateTime>();

        public void Mark()
            => _marks.Add(DateTime.Now);
    }
}
