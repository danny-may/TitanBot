using System;
using TitanBot.Storage;

namespace TitanBot.Commands
{
    public class CommandRecord : IDbRecord
    {
        public ulong Id { get; set; }
        public ulong MessageId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong AuthorId { get; set; }
        public ulong? GuildId { get; set; }
        public string MessageContent { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string UserName { get; set; }
        public string ChannelName { get; set; }
        public string GuildName { get; set; }
        public string Prefix { get; set; }
        public string CommandName { get; set; }
    }
}
