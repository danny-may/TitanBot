using Discord;
using Discord.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Extensions
{
    public static class IMessageChannelExtensions
    {
        public static async Task<IUserMessage> SendMessageSafeAsync(this IMessageChannel channel, string text, Func<Exception, Task> handler, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            Exception latest = null;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var serialised = JsonConvert.SerializeObject(embed, Formatting.Indented);
                    if (text.Length < 2000 && 
                            (embed == null || 
                                (embed.Fields.Length <= 25 && 
                                 embed.Fields.All(f => f.Name.Length <= 256 && f.Value.Length <= 1024) &&
                                 ((embed.Footer?.Text ?? "") + embed.Description).Length <= 2048 &&
                                 serialised.Length <= 4000)))
                        return await channel.SendMessageAsync(text, isTTS, embed, options);

                    using (var ms = new MemoryStream())
                    {
                        using (var sw = new StreamWriter(ms))
                        {
                            sw.Write(text);
                            if (embed != null)
                                sw.Write(serialised);
                            sw.Flush();
                            ms.Position = 0;
                            return await channel.SendFileAsync(ms, "Output.txt", $"{Res.Str.ErrorText} I tried to send a message that was too long!", isTTS, options);
                        }
                    }
                }
                catch (HttpException ex)
                {
                    latest = ex;
                    if (ex.HttpCode != (HttpStatusCode)500)
                        break;
                }
                catch (Exception ex)
                {
                    latest = ex;
                    break;
                }
            }
            await (handler?.Invoke(latest) ?? Task.CompletedTask);
            return null;
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
