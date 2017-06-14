using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Database;

namespace TitanBotBase.Settings
{
    class Setting : IDbRecord
    {
        public ulong Id { get; set; }
        public string Type { get; set; }
        public string Serialized { get; set; }
    }
}
