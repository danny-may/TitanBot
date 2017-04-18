using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace TitanBot2.Database.Models
{
    public class CmdPerm
    {
        [BsonId]
        public int entryId { get; set; }
        public ulong guildId { get; set; }
        public string commandname { get; set; }
        public ulong[] roleIds { get; set; }
        public ulong? permissionId { get; set; }
    }
}
