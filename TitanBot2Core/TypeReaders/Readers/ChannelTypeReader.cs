using Discord;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class ChannelTypeReader<T> : TypeReader
        where T : class, IChannel
    {
        public override async Task<TypeReaderResponse> Read(CmdContext context, string input)
        {
            if (context.Guild != null)
            {
                var results = new Dictionary<ulong, TypeReaderValue>();
                var channels = context.Guild.Channels;
                ulong id;

                //By Mention (1.0)
                if (MentionUtils.TryParseChannel(input, out id))
                    AddResult(results, channels.Where(c => c.Id == id).FirstOrDefault() as T, 1.0f);

                //By Id (0.9)
                if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    AddResult(results, channels.Where(c => c.Id == id).FirstOrDefault() as T, 0.90f);

                //By Name (0.7-0.8)
                foreach (var channel in channels.Where(x => string.Equals(input, x.Name, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, channel as T, channel.Name == input ? 0.80f : 0.70f);

                if (results.Count > 0)
                    return TypeReaderResponse.FromSuccess(results.Values);
            }

            return TypeReaderResponse.FromError("Channel not found.");
        }

        private void AddResult(Dictionary<ulong, TypeReaderValue> results, T channel, float score)
        {
            if (channel != null && !results.ContainsKey(channel.Id))
                results.Add(channel.Id, new TypeReaderValue(channel, score));
        }
    }
}
