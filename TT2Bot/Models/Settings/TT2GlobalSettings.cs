using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT2Bot.Models
{
    class TT2GlobalSettings
    {
        public string ImageRegex { get; set; }
        public ulong BotBugChannel { get; set; }
        public ulong BotSuggestChannel { get; set; }
        public ulong GHFeedbackChannel { get; set; }
    }
}
