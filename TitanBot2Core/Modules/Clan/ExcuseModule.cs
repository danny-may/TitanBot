using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Preconditions;
using TitanBot2.Services.Database.Models;

namespace TitanBot2.Modules.Clan
{
    public partial class ClanModule
    {
        [Group("Excuse")]
        [Summary("Provides a random user submitted excuse for not hitting the boss")]
        [RequireCustomPermission(0)]
        public class ExcuseModule : TitanBotModule
        {
            [Command]
            [Remarks("Displays an excuse for you, or the user you specified.")]
            public async Task ExcuseMeAsync(IUser user = null)
            {
                user = user ?? Context.User;

                var excuse = await Context.Database.Excuses.GetRandom();
                var submitter = Context.Client.GetUser(excuse.CreatorId);

                var builder = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        IconUrl = user.GetAvatarUrl(),
                        Name = $"{user.Username}#{user.Discriminator}"
                    },
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"Excuse #{excuse.ExcuseNo}",
                    },
                    Timestamp = DateTime.Now.AddSeconds(-new Random().Next(120, 3600)),
                    Color = System.Drawing.Color.Green.ToDiscord(),
                    Description = excuse.ExcuseText
                };
                await ReplyAsync($"<@{user.Id}> didnt attack the boss because: ", embed: builder);
            }

            [Command("Add")]
            [Remarks("Adds a new excuse for it to randomly choose from.")]
            public async Task AddExcuseAsync([Remainder]string text)
            {
                var excuse = new Excuse
                {
                    CreatorId = Context.User.Id,
                    ExcuseText = text,
                    SubmissionTime = DateTime.Now
                };
                await Context.Database.Excuses.Upsert(excuse);

                var excuseId = excuse.ExcuseNo;

                await ReplyAsync($"{Res.Str.SuccessText} Added the excuse as #{excuseId}");
            }

            [Command("Remove")]
            [Remarks("Removes an excuse you created")]
            public async Task RemoveExcuseAsync(int id)
            {
                var excuse = await Context.Database.Excuses.Get(id);

                if (excuse == null)
                {
                    await ReplyAsync($"{Res.Str.ErrorText} There is no excuse with that ID");
                    return;
                }

                if (Context.User.Id != Context.TitanBot.Owner.Id && Context.User.Id != excuse.CreatorId)
                {
                    await ReplyAsync($"{Res.Str.ErrorText} You do not own this excuse.");
                    return;
                }

                await Context.Database.Excuses.Delete(excuse);

                await ReplyAsync($"{Res.Str.SuccessText} Removed excuse #{excuse.ExcuseNo}");
            }
        }
    }
}
