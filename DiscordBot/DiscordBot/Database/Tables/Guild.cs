using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Database.Tables
{
    public class Guild : IBotDbRecord
    {
        public ulong GuildId { get; set; }
        public string TestText { get; set; }
    }
}
