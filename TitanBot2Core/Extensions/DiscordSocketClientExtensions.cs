using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TitanBot2.Extensions
{
    public static class DiscordSocketClientExtensions
    {
        public static async Task SendToAll(this DiscordSocketClient client, IEnumerable<ulong> channelIds, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            try
            {
                await channelIds.Select(c => client.GetChannel(c))
                                .Where(c => c is IMessageChannel)
                                .Cast<IMessageChannel>()
                                .SendToAll(text, isTTS, embed, options);
            }
            catch
            {

            }
        }

        public static IMessageChannel GetMessageChannelSafe(this DiscordSocketClient client, ulong channelId)
        {
            return client.ChannelExists(channelId) ? client.GetChannel(channelId) as IMessageChannel : null;
        }

        public static async Task<IMessage> GetMessageSafe(this DiscordSocketClient client, ulong channelId, ulong messageId)
        {
            var channel = client.GetChannel(channelId) as IMessageChannel;
            if (channel == null)
                return null;
            try
            {
                var message = await channel.GetMessageAsync(messageId);

                return message;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public static bool ChannelExists(this DiscordSocketClient client, ulong channelid)
        {
            return AllChannels(client).Select(c => c.Id).Contains(channelid);
        }

        public static IEnumerable<IChannel> AllChannels(this DiscordSocketClient client)
        {
            return client.Guilds.SelectMany(g => g.Channels).Cast<IChannel>()
                         .Concat(client.DMChannels)
                         .Concat(client.PrivateChannels)
                         .Concat(client.GroupChannels);
        }
    }
}
