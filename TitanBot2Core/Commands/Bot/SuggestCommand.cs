using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Bot
{
    public class SuggestCommand : Command
    {
        public SuggestCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => SuggestAsync((string)a[0]))
                 .WithArgTypes(typeof(string))
                 .WithItemAsParams(0);

            Usage.Add("`{0} <suggestion>` - Sends a suggestion to my home guild.");
            Description = "Allows you to make suggestions and feature requests for me!";
        }

        private async Task SuggestAsync(string message)
        {
            if (Context.SuggestionChannel == null)
            {
                await ReplyAsync("I could not find where I need to send the suggestion! Please try again later.", ReplyType.Error);
                return;
            }

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = $"{Context.User.Username}#{Context.User.Discriminator}",
                    IconUrl = Context.User.GetAvatarUrl()
                },
                Timestamp = DateTime.Now,
                Color = System.Drawing.Color.ForestGreen.ToDiscord()
            }
            .AddField("Suggestion", message)
            .AddInlineField(Context.Guild?.Name ?? Context.User.Username, Context.Guild?.Id ?? Context.User.Id)
            .AddInlineField(Context.Channel.Name, Context.Channel.Id);
            await Context.SuggestionChannel.SendMessageSafeAsync("", embed: builder.Build());
            await ReplyAsync("Suggestion sent", ReplyType.Success);
        }
    }
}
