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
            [CallFlag('i', "id", "Specifies an ID to use")]int? excuseId = null)
        {
            user = user ?? Author;

            if (user?.Id == BotUser.Id)
            {
                await ReplyAsync("Haha! You must be mistaken, I never miss a Titan Lord attack.");
                return;
            }

            var excuse = GetExcuse(excuseId, true, out int finalid);

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
                    Text = $"Excuse #{finalid}",
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

            await ReplyAsync($"Added the excuse as #{Database.Find<Excuse>(r => true).Result.Count(r => !r.Removed) - 1}", ReplyType.Success);
        }

        [Call("Remove")]
        [Usage("Removes an excuse you made by ID")]
        public async Task RemoveExcuseAsync(int id)
        {
            var excuse = GetExcuse(id, false, out int finalid);

            if (excuse == null || id != finalid)
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

            await ReplyAsync($"Removed excuse #{finalid}", ReplyType.Success);
        }

        private Excuse GetExcuse(int? id, bool allowRandom, out int finalId)
        {
            finalId = 0;
            Func<Excuse> def = () => new Excuse { CreatorId = Author.Id, Id = 0, ExcuseText = "Im uninteresting and havent made any excuses yet", SubmissionTime = DateTime.MinValue };
            var excuses = Database.Find<Excuse>(r => true).Result.ToArray();
            if (excuses.Length == 0)
                return def();
            if (id != null)
                if (excuses.Length > id.Value && !excuses[id.Value].Removed)
                {
                    finalId = id.Value;
                    return excuses[id.Value];
                }
            var valid = excuses.Where(e => !e.Removed).ToArray();
            if (valid.Length == 0 || !allowRandom)
                return def();
            var final = valid[new Random((int)(DateTime.UtcNow.Ticks % int.MaxValue)).Next(valid.Length)];
            finalId = excuses.ToList().IndexOf(final);
            return final;
                    
        }
    }

}
