using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TitanBot.Logging;

namespace TitanBot.Util
{
    public static class DiscordUtil
    {
        public static ILoggable ToLoggable(this Discord.LogMessage log)
            => new LogEntry(Logging.LogSeverity.Critical, LogType.Client, log.Message, log.Source);

        public static Color ToDiscord(this System.Drawing.Color color)
            => new Color(color.R, color.G, color.B);

        public static IEnumerable<IChannel> AllChannels(this DiscordSocketClient client)
        {
            return client.Guilds.SelectMany(g => g.Channels).Cast<IChannel>()
                         .Concat(client.DMChannels)
                         .Concat(client.PrivateChannels)
                         .Concat(client.GroupChannels);
        }

        public static async Task<IUserMessage> SendMessageSafeAsync(this IMessageChannel channel, string text, Func<Exception, Task> handler, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            Exception latest = null;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var serialised = JsonConvert.SerializeObject(embed, Newtonsoft.Json.Formatting.Indented);
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
                            return await channel.SendFileAsync(ms, "Output.txt", "I tried to send a message that was too long!", isTTS, options);
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

        public static bool HasCharPrefix(this IUserMessage msg, char c, out int argPos)
        {
            var text = msg.Content;
            if (text.Length > 0 && text[0] == c)
            {
                argPos = 1;
                return true;
            }
            argPos = -1;
            return false;
        }
        public static bool HasStringPrefix(this IUserMessage msg, string str, out int argPos, StringComparison comparisonType = StringComparison.Ordinal)
        {
            var text = msg.Content;
            if (text.StartsWith(str, comparisonType))
            {
                argPos = str.Length;
                return true;
            }
            argPos = -1;
            return false;
        }
        public static bool HasMentionPrefix(this IUserMessage msg, IUser user, out int argPos)
        {
            argPos = -1;
            var text = msg.Content;
            if (text.Length <= 3 || text[0] != '<' || text[1] != '@') return false;

            int endPos = text.IndexOf('>');
            if (endPos == -1) return false;
            if (text.Length < endPos + 2 || text[endPos + 1] != ' ') return false; //Must end in "> "

            ulong userId;
            if (!MentionUtils.TryParseUser(text.Substring(0, endPos + 1), out userId)) return false;
            if (userId == user.Id)
            {
                argPos = endPos + 2;
                return true;
            }
            return false;
        }

        public static bool HasAll(this GuildPermissions current, GuildPermissions requireAll)
            => Enum.GetValues(typeof(GuildPermission))
                   .Cast<GuildPermission>()
                   .Where(p => requireAll.Has(p))
                   .All(p => current.Has(p));

        public static bool HasAll(this GuildPermissions current, ulong requireAll)
            => current.HasAll(new GuildPermissions(requireAll));

        public static bool HasAll(this IGuildUser user, GuildPermissions requireAll)
            => user.GuildPermissions.HasAll(requireAll);

        public static bool HasAll(this IGuildUser user, ulong requireAll)
            => user.GuildPermissions.HasAll(new GuildPermissions(requireAll));
    }
}
