using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services;
using TitanBot2.Services.Database;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Common
{
    public class TitanbotDependencies
    {
        public TitanBot TitanBot { get; set; }
        public DiscordSocketClient Client { get; set; }
        public TitanbotDatabase Database { get; set; }
        public TimerService TimerService { get; set; }
        public Logger Logger { get; set; }
        public CachedWebClient WebClient { get; set; }
        public TT2DataService TT2DataService { get; set; }
        public ulong SuggestionChannelID { get; set; }
        public ulong BugChannelID { get; set; }
        public IMessageChannel SuggestionChannel
        { get
            {
                if (_suggestionChannel == null || _suggestionChannel.Id != SuggestionChannelID)
                    _suggestionChannel = Client.GetChannel(SuggestionChannelID) as IMessageChannel;
                return _suggestionChannel;
            }
        }
        public IMessageChannel BugChannel
        {
            get
            {
                if (_bugChannel == null || _bugChannel.Id != BugChannelID)
                    _bugChannel = Client.GetChannel(BugChannelID) as IMessageChannel;
                return _bugChannel;
            }
        }
        private IMessageChannel _bugChannel;
        private IMessageChannel _suggestionChannel;
    }
}
