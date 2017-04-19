using LiteDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Database.Models
{
    public class Timer
    {
        [BsonId]
        public int Id { get; set; }
        public ulong Guildid { get; set; }
        public ulong Userid { get; set; }
        public DateTime Start { get; set; }
        public DateTime Completion { get; set; }
        public string CompletionCallback { get; set; }
        public uint? Period { get; set; }
        public string PeriodCallback { get; set; }
        public string _custArgs { get; set; }
        public bool Complete { get; set; }

        [BsonIgnore]
        public JObject CustArgs
        {
            get
            {
                return JObject.Parse(_custArgs ?? "{}");
            }
            set
            {
                _custArgs = value.ToString();
            }
        }
    }
}
