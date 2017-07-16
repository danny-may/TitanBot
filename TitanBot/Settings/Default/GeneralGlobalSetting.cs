using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Settings
{
    public class GeneralGlobalSetting
    {
        public string DefaultPrefix { get; set; } = "t$";
        public string Token { get; set; }
        public ulong[] Owners { get; set; } = new ulong[0];
        public ulong PreferredPermission { get; set; } = 8;
    }
}
