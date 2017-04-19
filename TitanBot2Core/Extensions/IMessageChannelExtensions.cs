using Discord;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using TitanBot2.Common;
using System.Collections.Generic;

namespace TitanBot2.Extensions
{
    public static class IMessageChannelExtensions
    {
        public static async Task<IUserMessage> SendMessageSafeAsync(this IMessageChannel channel, string text, Func<Exception, Task> handler, bool isTTS = false, Embed embed = null, RequestOptions options = null)
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
                    return await channel.SendFileAsync(ms, "Output.txt", $"{Res.Str.ErrorText} I tried to send a message that was too long!", isTTS, options);
                }
            }
            catch (HttpException ex)
            {
                await (handler?.Invoke(ex) ?? Task.CompletedTask);
                return null;
            }
        }

        public static async Task<IUserMessage> SendMessageSafeAsync(this IMessageChannel channel, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return await channel.SendMessageSafeAsync(text, null, isTTS, embed, options);
        }

        public static async Task ModifySafeAsync(this IUserMessage msg, Action<MessageProperties> func, Func<Exception, Task> handler, RequestOptions options = null)
        {
            try
            {
                await msg.ModifyAsync(func, options);
            }
            catch (Exception ex)
            {
                await (handler?.Invoke(ex) ?? Task.CompletedTask);
            }
        }

        public static async Task ModifySafeAsync(this IUserMessage msg, Action<MessageProperties> func, RequestOptions options = null)
        {
            await msg.ModifySafeAsync(func, null, options);
        }

        public static async Task SendToAll(this IEnumerable<IMessageChannel> channels, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            foreach (var channel in channels)
            {
                await SendMessageSafeAsync(channel, text, isTTS, embed, options);
            }
        }

        public static bool UserHasPermission(this IGuildChannel channel, IGuildUser user, ChannelPermission permission)
            => user.GetPermissions(channel).Has(permission);
    }
}
