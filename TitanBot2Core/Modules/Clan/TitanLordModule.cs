﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Preconditions;
using TitanBot2.Services.Scheduler;
using TitanBot2.Extensions;
using TitanBot2.Services.Database.Models;
using TitanBot2.TypeReaders;
using Newtonsoft.Json.Linq;
using TitanBot2.Common;

namespace TitanBot2.Modules.Clan
{
    public partial class ClanModule
    {
        [Group("TitanLord"), Alias("TL", "Boss")]
        [RequireCustomPermission(8)]
        [RequireContext(ContextType.Guild)]
        class TitanLordModule : TitanBotModule
        {
            private async Task CompleteExisting(bool titanLordTicks = true, bool titanLordRounds = true, bool titanLordNows = true)
            {
                var existingTimers = new List<Timer>();

                if (titanLordTicks)
                    existingTimers = existingTimers.Concat(await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordTick)).ToList();

                foreach (var timer in existingTimers)
                {
                    var tickMessageId = (ulong?)timer.CustArgs[Callbacks.timerMessageId];
                    var tickMessageChannelId = (ulong?)timer.CustArgs[Callbacks.timerMessageChannelId];

                    if (tickMessageId == null || tickMessageChannelId == null)
                        continue;

                    await Context.Guild.DeleteMessage(tickMessageChannelId.Value, tickMessageId.Value);
                }

                if (titanLordRounds)
                    existingTimers = existingTimers.Concat((await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordRound)).ToList()).ToList();
                if (titanLordNows)
                    existingTimers = existingTimers.Concat((await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordNow)).ToList()).ToList();

                await Context.Database.Timers.Complete(existingTimers);
            }

            private async Task NewBoss(TimeSpan time)
            {
                var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
                guildData.TitanLord.CQ += 1;

                await Context.Database.Guilds.Update(guildData);

                var bossHp = Calculator.TitanLordHp(guildData.TitanLord.CQ);
                var clanBonus = Calculator.ClanBonus(guildData.TitanLord.CQ);
                var advStart = Calculator.AdvanceStart(guildData.TitanLord.CQ);

                var latestTimer = await Context.Database.Timers.GetLatest(Context.Guild.Id, EventCallback.TitanLordNow, true);

                var builder = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        IconUrl = Res.Emoji.Information_source,
                        Name = "Titan Lord data updated!"
                    },
                    ThumbnailUrl = "https://cdn.discordapp.com/attachments/275257967937454080/308047011289235456/emoji.png",
                    Color = System.Drawing.Color.DarkOrange.ToDiscord(),
                    Timestamp = DateTime.Now,
                };

                builder.AddField("New Clan Quest", guildData.TitanLord.CQ);
                builder.AddField("New bonus", clanBonus.Beautify());
                builder.AddField("Next Titan Lord HP", bossHp.Beautify());
                builder.AddField("Time to kill", (DateTime.Now.Add(time).AddHours(-6) - latestTimer.To).Value.Beautify());

                await ReplyAsync("", embed: builder.Build());
            }

            [Command(RunMode = RunMode.Async)]
            [Remarks("FALSE")]
            public async Task DefaultCommand()
            {
                await ReplyAsync($"{Res.Str.ErrorText} You havent supplied enough arguments. Please use `{Context.Prefix}help <command>` for usage info");
            }

            [Command("Now", RunMode = RunMode.Async)]
            [Remarks("Notifys everyone that the Titan Lord is ready to be killed right now, and starts hourly pings.")]
            public async Task TitanLordNowAsync()
            {
                await CompleteExisting();

                var time = DateTime.Now;

                var timer = new Timer
                {
                    GuildId = Context.Guild.Id,
                    UserId = Context.User.Id,
                    ChannelId = Context.Channel.Id,
                    MessageId = Context.Message.Id,
                    Callback = EventCallback.TitanLordRound,
                    From = time.AddHours(1),
                    SecondInterval = 60 * 60,
                    To = time.AddDays(1)
                };

                await Callbacks.TitanLordNow(new TimerContext(Context.Dependencies, timer, DateTime.Now));

                await Context.Database.Timers.Add(timer);

                await ReplyAsync($"{Res.Str.SuccessText} Ill let everyone know now!");
            }

            [Command("Stop", RunMode = RunMode.Async)]
            [Remarks("Stops any currently running timers")]
            public async Task StopAsync()
            {
                await CompleteExisting();
                await ReplyAsync($"{Res.Str.SuccessText} Stopped all existing timers");
            }

            [Command("Dead", RunMode = RunMode.Async)]
            [Remarks("Sets a timer for 6 hours")]
            public async Task DeadAsync()
                => await TitanLordInAsync(new TimeSpan(6, 0, 0));

            [Command("When", RunMode = RunMode.Async)]
            [Remarks("States how long until the next titan Lord")]
            public async Task WhenAsync()
            {
                var activeTimer = await Context.Database.Timers.GetLatest(Context.Guild.Id, EventCallback.TitanLordNow);

                if (activeTimer == null)
                    await ReplyAsync($"{Res.Str.ErrorText} There are no timers currently running");
                else
                    await ReplyAsync($"{Res.Str.SuccessText} There will be a TitanLord in {(activeTimer.To.Value - DateTime.Now).Beautify()}");
            }

            [Command("In", RunMode = RunMode.Async)]
            [Remarks("Sets a timer running for the given time for alerting when the boss is up.")]
            public async Task TitanLordInAsync([OverrideTypeReader(typeof(BetterTimespanTypeReader))]TimeSpan time)
            {
                var roundsRunning = (await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordRound)).Count();
                var timersRunning = (await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordNow)).Count();

                await CompleteExisting();

                if (roundsRunning > 0 && timersRunning == 0)
                    await NewBoss(time);

                var timeNow = DateTime.Now;

                var tickTimer = new Timer
                {
                    GuildId = Context.Guild.Id,
                    UserId = Context.User.Id,
                    ChannelId = Context.Channel.Id,
                    MessageId = Context.Message.Id,
                    Callback = EventCallback.TitanLordTick,
                    From = timeNow,
                    SecondInterval = 10,
                    To = timeNow.Add(time)
                };
                var nowTimer = new Timer
                {
                    GuildId = Context.Guild.Id,
                    UserId = Context.User.Id,
                    ChannelId = Context.Channel.Id,
                    MessageId = Context.Message.Id,
                    Callback = EventCallback.TitanLordNow,
                    From = timeNow.Add(time),
                    SecondInterval = 1,
                    To = timeNow.Add(time)
                };
                var roundTimer = new Timer
                {
                    GuildId = Context.Guild.Id,
                    UserId = Context.User.Id,
                    ChannelId = Context.Channel.Id,
                    MessageId = Context.Message.Id,
                    Callback = EventCallback.TitanLordRound,
                    From = timeNow.Add(time).AddHours(1),
                    SecondInterval = 60 * 60,
                    To = timeNow.Add(time).AddDays(1)
                };

                var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
                var tlChannel = Context.Channel;
                if (guildData.TitanLord?.Channel != null)
                    tlChannel = Context.Guild.GetTextChannel(guildData.TitanLord.Channel.Value) ?? tlChannel;

                var message = await tlChannel.SendMessageSafeAsync("Loading Timer...");

                if (guildData.TitanLord.PinTimer && (Context.User as IGuildUser).GuildPermissions.Has(GuildPermission.ManageMessages))
                    await message.PinAsync();

                var custArgs = new JObject();

                custArgs.Add(Callbacks.timerMessageId, message.Id);
                custArgs.Add(Callbacks.timerMessageChannelId, tlChannel.Id);

                tickTimer.CustArgs = custArgs;
                nowTimer.CustArgs = custArgs;

                await Context.TimerService.AddTimers(new Timer[] { tickTimer, nowTimer, roundTimer });

                await ReplyAsync($"{Res.Str.SuccessText} Started a timer for **{time}**");
            }
        }
    }
}
