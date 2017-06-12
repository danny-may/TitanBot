using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Database
{
    public class UserSetting : IDbRecord
    {
        public ulong Id { get; set; }
        public bool AltFormat { get; set; } = true;
    }
}
