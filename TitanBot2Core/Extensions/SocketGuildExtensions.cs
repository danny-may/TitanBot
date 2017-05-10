using Discord.WebSocket;
using System.Threading.Tasks;

namespace TitanBot2.Extensions
{
    public static class SocketGuildExtensions
    {
        public static async Task DeleteMessage(this SocketGuild guild, ulong channelId, ulong messageId)
        {
            var channel = guild.GetTextChannel(channelId);
            if (channel == null)
                return;

            var message = await channel.GetMessageAsync(messageId);
            if (message == null)
                return;

            await message.DeleteAsync();
        }
    }
}
