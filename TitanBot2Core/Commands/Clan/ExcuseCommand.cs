using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.Database.Models;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Clan
{
    public class ExcuseCommand : Command
    {
        public ExcuseCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ExcuseUserAsync());
            Calls.AddNew(a => ExcuseUserAsync((IUser)a[0]))
                 .WithArgTypes(typeof(IUser));
            Calls.AddNew(a => AddExcuseAsync((string)a[0]))
                 .WithArgTypes(typeof(string))
                 .WithSubCommand("Add")
                 .WithItemAsParams(0);
            Calls.AddNew(a => RemoveExcuseAsync((int)a[0]))
                 .WithArgTypes(typeof(int))
                 .WithSubCommand("Remove");
        }

        public async Task ExcuseUserAsync(IUser user = null)
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

        public async Task AddExcuseAsync(string text)
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
