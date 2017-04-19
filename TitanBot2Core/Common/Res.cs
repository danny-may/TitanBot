using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using TitanBot2.Extensions;

namespace TitanBot2.Common
{
    public static class Res
    {
        public static class Str
        {
            public static string ErrorText { get; } = ":no_entry_sign: **Oops!**";
            public static string SuccessText { get; } = ":white_check_mark: **Got it!**";
            public static string InfoText { get; } = ":information_source:";
            
        }

        public static class Emoji
        {

            public static string No_entry_sign { get; } = "http://emojipedia-us.s3.amazonaws.com/cache/74/95/7495a6b391014cb87772e381ab24d22a.png";
            public static string White_check_mark { get; } = "http://emojipedia-us.s3.amazonaws.com/cache/f7/cd/f7cd0427e5531771b69f6cce997cb872.png";
            public static string Information_source { get; } = "http://emojipedia-us.s3.amazonaws.com/cache/2f/e4/2fe4dd7dea335d509e7f03ac620847d8.png";
        }

        public static class Embeds
        {
            public static Embed BuildAliveNotification(IUser bot, string reason)
            {
                var builder = GetBaseNotification(bot);
                builder.Title = "Online";
                builder.Description = "---------------";
                builder.AddInlineField("Offline Reason", reason ?? "None");
                builder.Color = System.Drawing.Color.LimeGreen.ToDiscord();

                return builder.Build();
            }

            public static Embed BuildDeadNotification(IUser bot, TimeSpan? delay, string reason)
            {
                var builder = GetBaseNotification(bot);
                builder.Title = $"Shutting down";
                builder.Description = "---------------";
                builder.AddInlineField("Time until shutdown", delay?.ToString() ?? "Now");
                builder.AddInlineField("Shutdown Reason", reason ?? "None");
                builder.Color = System.Drawing.Color.Red.ToDiscord();

                return builder.Build();
            }

            public static EmbedBuilder GetBaseNotification(IUser bot)
            {
                return new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "Bot Notification",
                        IconUrl = Res.Emoji.Information_source
                    },
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = bot.GetAvatarUrl(),
                        Text = bot.Username
                    },
                    Timestamp = DateTime.Now,
                };
            }
        }
    }
}
