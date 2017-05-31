using Discord;
using System;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.Database.Tables;

namespace TitanBot2.Commands.Clan
{
    [Description("Missed the boss? Or did someone else? Use this to get a water-tight excuse whenever you need!")]
    public class ExcuseCommand : Command
    {
        [Call]
        [Usage("Gets an excuse for why that person (or yourself) didnt attack the boss")]
        [CallFlag(typeof(int?), "i", "id", "Specifies an ID to use")]
        public async Task ExcuseUserAsync(IUser user = null)
        {
            user = user ?? Context.User;

            Flags.TryGet("i", out int? excuseId);

            if (user?.Id == Context.Client.CurrentUser.Id)
            {
                await ReplyAsync("Haha! You must be mistaken, I never miss a Titan Lord attack.");
                return;
            }
            Excuse excuse;
            if (excuseId == null)
                excuse = await Context.Database.Excuses.GetRandom();
            else
                excuse = await Context.Database.Excuses.Get(excuseId.Value) ?? await Context.Database.Excuses.GetRandom();

            excuse = excuse ?? new Excuse { CreatorId = Context.Client.CurrentUser.Id, ExcuseNo = 0, ExcuseText = "Im uninteresting and havent made any excuses yet", SubmissionTime = DateTime.MinValue };

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

        [Call("Add")]
        [Usage("Adds an excuse to the pool of available excuses")]
        public async Task AddExcuseAsync([Dense]string text)
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

        [Call("Remove")]
        [Usage("Removes an excuse you made by ID")]
        public async Task RemoveExcuseAsync(int id)
        { 
            var excuse = await Context.Database.Excuses.Get(id);

            if (excuse == null)
            {
                await ReplyAsync("There is no excuse with that ID", ReplyType.Error);
                return;
            }

            if (Context.User.Id != Context.BotClient.Owner.Id && Context.User.Id != excuse.CreatorId)
            {
                await ReplyAsync("You do not own this excuse.", ReplyType.Error);
                return;
            }

            await Context.Database.Excuses.Delete(excuse);

            await ReplyAsync($"Removed excuse #{excuse.ExcuseNo}", ReplyType.Success);
        }
    }
}
