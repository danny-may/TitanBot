using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            catch (Exception ex)
            {

            }
        }
    }
}
