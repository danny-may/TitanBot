using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.Database.Models
{
    public class User
    {
        [BsonId]
        public ulong DiscordId { get; set; }
        public string SupportCode { get; set; }
    }
}
