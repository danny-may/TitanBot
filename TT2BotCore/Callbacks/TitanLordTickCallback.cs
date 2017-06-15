using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Scheduler;
using TitanBotBase.Settings;
using TitanBotBase.Util;
using TT2BotCore.Models;

namespace TT2BotCore.Callbacks
{
    class TitanLordTickCallback : ISchedulerCallback
    {
        ISettingsManager SettingsManager { get; }
        DiscordSocketClient Client { get; }

        public TitanLordTickCallback(ISettingsManager manager, DiscordSocketClient client)
        {
            SettingsManager = manager;
            Client = client;
        }

        public void Handle(ISchedulerRecord record, DateTime eventTime)
        {
            if (record.GuildId == null)
                return;

            var data = JsonConvert.DeserializeObject<TitanLordTimerData>(record.Data);
            var settings = SettingsManager.GetGroup<TitanLordSettings>(record.GuildId.Value);

            var messageChannel = Client.GetChannel(data.MessageChannelId) as IMessageChannel;
            if (data.MessageId != 0)
            {
                var message = messageChannel?.GetMessageAsync(data.MessageId)?.Result as IUserMessage;

                message?.ModifySafeAsync(m => m.Content = Contextualise(settings.TimerText, settings, record, eventTime)).Wait();
            }

            foreach (var ping in settings.PrePings)
            {
                var delta = (record.EndTime - eventTime).Add(new TimeSpan(0, 0, -ping));
                if (delta < record.Interval && delta > new TimeSpan())
                    messageChannel?.SendMessageSafeAsync(Contextualise(settings.InXText, settings, record, eventTime)).Wait();
            }
        }

        public void Complete(ISchedulerRecord record, bool wasCancelled)
        {
            if (record.GuildId == null)
                return;

            var data = JsonConvert.DeserializeObject<TitanLordTimerData>(record.Data);
            var settings = SettingsManager.GetGroup<TitanLordSettings>(record.GuildId.Value);

            var messageChannel = Client.GetChannel(data.MessageChannelId) as IMessageChannel;
            if (data.MessageId != 0)
            {
                var message = messageChannel?.GetMessageAsync(data.MessageId)?.Result as IUserMessage;
                message?.DeleteAsync().Wait();
            }

            if (!wasCancelled)
                messageChannel?.SendMessageSafeAsync(Contextualise(settings.NowText, settings, record, record.EndTime)).Wait();
        }

        private static string Contextualise(string message, TitanLordSettings settings, ISchedulerRecord timer, DateTime eventTime)
        {
            var CQ = settings.CQ;
            var user = timer.UserId;
            var remaining = timer.EndTime - eventTime;
            var completesAt = timer.EndTime;
            var round = 0;

            return message.Replace("%CQ%", CQ.ToString())
                          .Replace("%USER%", $"<@{user}>")
                          .Replace("%TIME%", remaining.ToString())
                          .Replace("%ROUND%", round.ToString())
                          .Replace("%COMPLETE%", completesAt.ToShortTimeString());
        }
    }
}
