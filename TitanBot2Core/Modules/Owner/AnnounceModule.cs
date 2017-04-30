﻿using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;

namespace TitanBot2.Modules.Owner
{
    public partial class OwnerModule
    {
        [Group("Announce")]
        [Summary("Used to send notifications to every server the bot is on in order to announce big changes or updates")]
        class AnnounceModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            [Remarks("Sends an announcement to all guilds the bot is currently on.")]
            public async Task AnnounceAsync([Remainder, Summary("Text to display in the announcement")]string message)
            {
                var builder = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        IconUrl = Context.User.GetAvatarUrl(),
                        Name = Context.User.Username
                    },
                    Color = System.Drawing.Color.Aqua.ToDiscord(),
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                        Text = $"{Context.Client.CurrentUser.Username} | Announcement"
                    },
                    Title = "Global Announcement",
                    Description = message,
                    Timestamp = DateTime.Now
                };

                var guilds = Context.Client.Guilds;

                await Context.Channel.SendMessageAsync($"{Res.Str.SuccessText} I will notify all my users!");

                foreach (var guild in guilds)
                {
                    if (guild.DefaultChannel.UserHasPermission(Context.Guild.GetUser(Context.Client.CurrentUser.Id), ChannelPermission.SendMessages))
                        await guild.DefaultChannel.SendMessageSafeAsync(guild.Owner.Mention, embed: builder.Build());
                    else
                    {
                        var dmChannel = (IDMChannel)Context.Client.DMChannels.SingleOrDefault(c => c.Recipient.Id == guild.Owner.Id) ??
                            await guild.Owner.CreateDMChannelAsync();
                        await dmChannel.SendMessageSafeAsync($"I was unable to send this message in the default channel of {guild.Name} (#{guild.DefaultChannel.Name})", embed: builder.Build());
                    }
                }

                await Task.Delay(20000);
            }
        }
    }
}