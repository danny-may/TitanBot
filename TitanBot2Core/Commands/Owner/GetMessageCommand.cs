using Discord;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Owner
{
    [Description("Gets a single message")]
    [RequireOwner]
    class GetMessageCommand : Command
    {
        [Call]
        [Usage("Gets the given message from the current or given channel")]
        async Task GetMessageAsync(ulong messageId, ulong? channelId = null, bool escape = false)
        {
            var channel = Context.Client.GetChannel(channelId ?? Context.Channel.Id) as IMessageChannel;
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
