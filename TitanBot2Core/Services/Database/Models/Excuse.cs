using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.Database.Models
{
    public class Excuse
    {
        [BsonId]
        public int ExcuseNo { get; set; }
        public ulong CreatorId { get; set; }
        public string ExcuseText { get; set; }
        public DateTime SubmissionTime { get; set; }
    }
}
