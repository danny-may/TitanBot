using System.Collections.Generic;
using TitanBot.Storage;

namespace TitanBot.Settings
{
    class GroupingMap : IDbRecord
    {
        public ulong Id { get; set; }
        public Dictionary<int, string[]> Mapping { get; set; } = new Dictionary<int, string[]>();
    }
}
