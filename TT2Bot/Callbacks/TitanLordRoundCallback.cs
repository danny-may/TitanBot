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
using TT2Bot.Models;

namespace TT2Bot.Callbacks
{
    class TitanLordRoundCallback : ISchedulerCallback
    {
        ISettingsManager SettingsManager { get; }
        DiscordSocketClient Client { get; }

        public TitanLordRoundCallback(ISettingsManager manager, DiscordSocketClient client)
        {
            SettingsManager = manager;
            Client = client;
        }

        public void Complete(ISchedulerRecord record, bool wasCancelled)
        {
            if (!wasCancelled)
                Handle(record, record.EndTime);
        }

        public async void Handle(ISchedulerRecord record, DateTime eventTime)
        {
            if (record.GuildId == null)
                return;

            var data = JsonConvert.DeserializeObject<TitanLordTimerData>(record.Data);
            var settings = SettingsManager.GetGroup<TitanLordSettings>(record.GuildId.Value);

            var messageChannel = Client.GetChannel(data.MessageChannelId) as IMessageChannel;

            if (settings.RoundPings)
                await (messageChannel?.SendMessageSafeAsync(Contextualise(settings.RoundText, settings, record, eventTime)) ?? Task.CompletedTask);
        }

        private static string Contextualise(string message, TitanLordSettings settings, ISchedulerRecord timer, DateTime eventTime)
        {
            var CQ = settings.CQ;
            var user = timer.UserId;
            var remaining = 0;
            var completesAt = timer.StartTime;
            var round = 1 + (eventTime - timer.StartTime).Seconds / timer.Interval.Seconds;

            return message.Replace("%CQ%", CQ.ToString())
                          .Replace("%USER%", $"<@{user}>")
                          .Replace("%TIME%", remaining.ToString())
                          .Replace("%ROUND%", round.ToString())
                          .Replace("%COMPLETE%", completesAt.ToShortTimeString());
        }
    }
}
