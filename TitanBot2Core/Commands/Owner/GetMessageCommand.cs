using Discord;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    public class GetMessageCommand : Command
    {
        public GetMessageCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => GetMessageAsync(Context.Channel.Id, (ulong)a[0]))
                 .WithArgTypes(typeof(ulong));
            Calls.AddNew(a => GetMessageAsync(Context.Channel.Id, (ulong)a[0], (bool)a[1]))
                 .WithArgTypes(typeof(ulong), typeof(bool));
            Calls.AddNew(a => GetMessageAsync((ulong)a[0], (ulong)a[1]))
                 .WithArgTypes(typeof(ulong), typeof(ulong));
            Calls.AddNew(a => GetMessageAsync((ulong)a[0], (ulong)a[1], (bool)a[2]))
                 .WithArgTypes(typeof(ulong), typeof(ulong), typeof(bool));
            Description = "Gets a single message";
            Usage.Add("`{0} <messageId> [escaped]` - Gets the given message from the current channel");
            Usage.Add("`{0} <channelId> <messageId> [escaped]` - Gets the given message from the given channel");
            RequireOwner = true;
        }

        private async Task GetMessageAsync(ulong channelId, ulong messageId, bool escape = false)
        {
            var channel = Context.Client.GetChannel(channelId) as IMessageChannel;
            if (channel == null)
            {
                await ReplyAsync("I could not find that channel!", ReplyType.Error);
                return;
            }

            var message = await channel.GetMessageAsync(messageId);
            if (message == null)
            {
                await ReplyAsync("I could not find that message!", ReplyType.Error);
                return;
            }

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = message.Author.GetAvatarUrl(),
                    Name = $"{message.Author.Username}#{message.Author.Discriminator}"
                },
                Timestamp = message.Timestamp,
                Color = System.Drawing.Color.Green.ToDiscord(),
                Description = escape ? Format.Sanitize(message.Content) : message.Content
            };

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
