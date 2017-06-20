using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Util;
using TT2Bot.Models.Database;

namespace TT2Bot.Commands.Clan
{
    [Description("Missed the boss? Or did someone else? Use this to get a water-tight excuse whenever you need!")]
    class ExcuseCommand : Command
    {
        [Call]
        [Usage("Gets an excuse for why that person (or yourself) didnt attack the boss")]
        public async Task ExcuseUserAsync(IUser user = null,
            [CallFlag('i', "id", "Specifies an ID to use")]ulong? excuseId = null)
        {
            user = user ?? Author;

            if (user?.Id == BotUser.Id)
            {
                await ReplyAsync("Haha! You must be mistaken, I never miss a Titan Lord attack.");
                return;
            }

            var excuse = GetExcuse(excuseId, true);

            var submitter = Client.GetUser(excuse.CreatorId);

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = user.GetAvatarUrl(),
                    Name = $"{user.Username}#{user.Discriminator}"
                },
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Excuse #{excuse.Id}",
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
                CreatorId = Author.Id,
                ExcuseText = text,
                SubmissionTime = DateTime.Now
            };
            await Database.Insert(excuse);

            await ReplyAsync($"Added the excuse as #{excuse.Id}", ReplyType.Success);
        }

        [Call("Remove")]
        [Usage("Removes an excuse you made by ID")]
        public async Task RemoveExcuseAsync(ulong id)
        {
            var excuse = GetExcuse(id, false);

            if (excuse == null || excuse.Id == 0)
            {
                await ReplyAsync("There is no excuse with that ID", ReplyType.Error);
                return;
            }

            if (!Bot.Owners.Contains(Author.Id) && Author.Id != excuse.CreatorId)
            {
                await ReplyAsync("You do not own this excuse.", ReplyType.Error);
                return;
            }

            excuse.Removed = true;

            await Database.Upsert(excuse);

            await ReplyAsync($"Removed excuse #{excuse.Id}", ReplyType.Success);
        }

        private Excuse GetExcuse(ulong? id, bool allowRandom)
        {
            if (id.HasValue)
                return Database.FindById<Excuse>(id.Value).Result;
            var all = Database.Find<Excuse>(e => true).Result.ToArray();
            if (all.Length == 0)
                return new Excuse { CreatorId = Author.Id, Id = 0, ExcuseText = "Im uninteresting and havent made any excuses yet", SubmissionTime = DateTime.MinValue };
            return all[new Random().Next(all.Length)];
        }
    }

}
