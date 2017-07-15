using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Storage;
using TitanBot.Util;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("EXCEPTION_HELP_DESCRIPTION")]
    [RequireOwner]
    class ExceptionCommand : Command
    {
        [Call]
        [Usage("EXCEPTION_HELP_USAGE")]
        async Task ShowException(ulong exceptionId, [CallFlag('f', "full", "Returns the full exception in a file")]bool full = false)
        {
            var exception = await Database.FindById<Error>(exceptionId);
            if (exception == null)
            {
                await ReplyAsync("EXCEPTION_NOTFOUND", ReplyType.Error, exceptionId);
                return;
            }

            var user = Client.GetUser(exception.User ?? 0);
            var channel = Client.GetChannel(exception.Channel ?? 0) as IMessageChannel;
            var message = channel?.GetMessageAsync(exception.Message ?? 0)?.Result as IUserMessage;

            if (full)
            {
                var text = $"USER:\n{(user?.Username ?? "UNKNOWNUSER") + "#" + (user?.Discriminator ?? "0000")}\n" +
                           $"{TextResource.GetResource("EXCEPTION_CHANNEL")}:\n{channel.Name} ({channel.Id})\n";
                if (channel is IGuildChannel guildChannel)
                    text += $"{TextResource.GetResource("EXCEPTION_GUILD")}:\n{guildChannel.Guild.Name} ({guildChannel.Guild.Id})\n";
                text += $"{TextResource.GetResource("EXCEPTION_MESSAGE")}:\n{message.Content}\n\n";
                text += exception.Content;

                await Reply.WithAttachment(() => text.ToStream(), $"Exception{exceptionId}.txt")
                           .WithMessage(TextResource.Format("EXCEPTION_FULLMESSAGE", ReplyType.Success, exceptionId))
                           .SendAsync();
            }
            else
            {
                var builder = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        IconUrl = user?.GetAvatarUrl(),
                        Name = (user?.Username ?? "UNKNOWNUSER") + "#" + (user?.Discriminator ?? "0000")
                    },
                    Description = exception.Description,
                    Timestamp = exception.Time,
                    Color = System.Drawing.Color.Red.ToDiscord()
                }.AddField(TextResource.GetResource("EXCEPTION_MESSAGE"), message.Content)
                 .AddInlineField(TextResource.GetResource("EXCEPTION_CHANNEL"), $"{channel.Name} ({channel.Id})");
                if (channel is IGuildChannel guildChannel)
                    builder.AddInlineField(TextResource.GetResource("EXCEPTION_GUILD"), $"{guildChannel.Guild.Name} ({guildChannel.Guild.Id})");

                await ReplyAsync(Embedable.FromEmbed(builder));
            }
        }
    }
}
