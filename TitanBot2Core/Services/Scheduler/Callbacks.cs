using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;

namespace TitanBot2.Services.Scheduler
{
    public static class Callbacks
    {
        public const string defaultChannelId = "defaultChannelId";
        public const string timerMessageId = "timerMessageId";
        public const string timerMessageChannelId = "timerMessageChannelId";

        public static async Task TitanLordNow(TimerContext context)
        {
            if (context.Guild == null)
                return;
            var guildData = await context.Database.Guilds.GetGuild(context.Guild.Id);
            var tlChannel = context.Channel;
            if (guildData.TitanLord?.Channel != null)
                tlChannel = context.Guild.GetTextChannel(guildData.TitanLord.Channel.Value) ?? tlChannel;

            var messageText = FormatString(guildData.TitanLord?.NowText, new TimeSpan(0), context.User);

            var message = await tlChannel.SendMessageSafeAsync(messageText);

            var tickMessageId = (ulong?)context.Timer.CustArgs[timerMessageId];
            var tickMessageChannelId = (ulong?)context.Timer.CustArgs[timerMessageChannelId];

            if (tickMessageId == null || tickMessageChannelId == null)
                return;

            await (await context.Guild.GetTextChannel(tickMessageChannelId.Value).GetMessageAsync(tickMessageId.Value))?.DeleteAsync();
        }

        public static async Task TitanLordTick(TimerContext context)
        {
            if (context.Guild == null)
                return;

            var messageId = (ulong?)context.Timer.CustArgs[timerMessageId];
            var messageChannelId = (ulong?)context.Timer.CustArgs[timerMessageChannelId];

            var guildData = await context.Database.Guilds.GetGuild(context.Guild.Id);

            var timeRemaining = context.Timer.To.HasValue ? context.Timer.To.Value - context.EventTime : new TimeSpan();

            if (messageId != null && messageChannelId != null)
            {
                var message = await context.Client.GetMessageSafe(messageChannelId.Value, messageId.Value) as IUserMessage;
                if (message != null && message.Author.Id == context.Client.CurrentUser.Id)
                    await message.ModifySafeAsync(m => m.Content = FormatString(guildData.TitanLord.TimerText,
                                                                                timeRemaining,
                                                                                context.User));
            }

            var alertTimes = guildData.TitanLord.PrePings;

            foreach (var alert in alertTimes)
            {
                if (timeRemaining.TotalSeconds - alert < context.Timer.SecondInterval &&
                    timeRemaining.TotalSeconds - alert >= 0)
                {
                    var tlChannel = context.Channel;
                    if (guildData.TitanLord?.Channel != null)
                        tlChannel = context.Guild.GetTextChannel(guildData.TitanLord.Channel.Value) ?? tlChannel;
                    await tlChannel.SendMessageSafeAsync(FormatString(guildData.TitanLord.InXText, timeRemaining, context.User));
                }
            }
        }

        public static async Task TitanLordNextRound(TimerContext context)
        {
            if (context.Guild == null)
                return;

            var guildData = await context.Database.Guilds.GetGuild(context.Guild.Id);

            var tlChannel = context.Channel;
            if (guildData.TitanLord?.Channel != null)
                tlChannel = context.Guild.GetTextChannel(guildData.TitanLord.Channel.Value) ?? tlChannel;

            var round = (int)(context.EventTime - context.Timer.From).TotalSeconds / (60 * 60) + 2;

            await tlChannel.SendMessageSafeAsync(FormatString(guildData.TitanLord.RoundText, new TimeSpan(), context.User, round));
        }

        public static string FormatString(string text, TimeSpan time, IUser user, int round = 0)
        {
            return text.Replace("%TIME%", time.ToString())
                       .Replace("%USER%", user.Mention)
                       .Replace("%ROUND%", round.ToString());
        }
    }
}
