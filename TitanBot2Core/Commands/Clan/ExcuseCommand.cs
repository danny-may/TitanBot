using Discord;
using System;
using System.Threading.Tasks;
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
            Calls.AddNew(a => ExcuseUserAsync((IUser)a[0], (int)a[1]))
                 .WithArgTypes(typeof(IUser), typeof(int));
            Calls.AddNew(a => AddExcuseAsync((string)a[0]))
                 .WithArgTypes(typeof(string))
                 .WithSubCommand("Add")
                 .WithItemAsParams(0);
            Calls.AddNew(a => RemoveExcuseAsync((int)a[0]))
                 .WithArgTypes(typeof(int))
                 .WithSubCommand("Remove");
            Usage.Add("`{0} [user] [id]` - Gets an excuse for why that person (or yourself) didnt attack the boss");
            Usage.Add("`{0} add <text>` - Adds an excuse to the pool of available excuses");
            Usage.Add("`{0} remove <id>` - Removes an excuse you made by ID");
            Description = "Missed the boss? Or did someone else? Use this to get a water-tight excuse whenever you need!";
        }

        public async Task ExcuseUserAsync(IUser user = null, int? excuseId = null)
        {
            user = user ?? Context.User;

            if (user?.Id == Context.Client.CurrentUser.Id)
            {
                await ReplyAsync("Haha! You must be mistaken, I never miss a Titan Lord attack.");
                return;
            }
            Excuse excuse;
            if (excuseId == null)
                excuse = await Context.Database.Excuses.GetRandom();
            else
                excuse = await Context.Database.Excuses.Get(excuseId??0);
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

            await ReplyAsync($"Added the excuse as #{excuseId}", ReplyType.Success);
        }

        public async Task RemoveExcuseAsync(int id)
        { 
            var excuse = await Context.Database.Excuses.Get(id);

            if (excuse == null)
            {
                await ReplyAsync("There is no excuse with that ID", ReplyType.Error);
                return;
            }

            if (Context.User.Id != Context.TitanBot.Owner.Id && Context.User.Id != excuse.CreatorId)
            {
                await ReplyAsync("You do not own this excuse.", ReplyType.Error);
                return;
            }

            await Context.Database.Excuses.Delete(excuse);

            await ReplyAsync($"Removed excuse #{excuse.ExcuseNo}", ReplyType.Success);
        }
    }
}
