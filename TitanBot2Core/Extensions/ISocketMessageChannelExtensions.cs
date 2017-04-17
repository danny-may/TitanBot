using Discord;
using Discord.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Extensions
{
    public static class ISocketMessageChannelExtensions
    {
        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel, string text, Func<Exception, Task> handler = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            try
            {
                if (text.Length < 2000)
                    return await channel.SendMessageAsync(text, isTTS, embed, options);

                using (var ms = new MemoryStream())
                {
                    using (var sw = new StreamWriter(ms))
                    {
                        sw.Write(text);
                        sw.Flush();
                    }
                    ms.Position = 0;
                    return await channel.SendFileAsync(ms, "Output.txt", $"{Resources.Str.ErrorText} I tried to send a message that was too long!", isTTS, options);
                }
            }
            catch (HttpException ex)
            {
                await (handler?.Invoke(ex) ?? Task.CompletedTask);
                return null;
            }
        }

        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return await channel.SendMessageAsync(text, ex => Task.CompletedTask, isTTS, embed, options);
        }
    }
}
