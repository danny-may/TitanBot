using Discord;
using System.Threading.Tasks;
using TitanBot.Replying;
using TitanBot.Storage;
using TitanBot.Util;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(Desc.EXCEPTION)]
    [RequireOwner]
    class ExceptionCommand : Command
    {
        [Call]
        [Usage(Usage.EXCEPTION)]
        async Task ShowException(ulong exceptionId, [CallFlag('f', "full", Flags.EXCEPTION_F)]bool full = false)
        {
            var exception = await Database.FindById<Error>(exceptionId);
            if (exception == null)
            {
                await ReplyAsync(ExceptionText.NOTFOUND, ReplyType.Error, exceptionId);
                return;
            }

            var user = Client.GetUser(exception.User ?? 0);
            var channel = Client.GetChannel(exception.Channel ?? 0) as IMessageChannel;
            var message = channel?.GetMessageAsync(exception.Message ?? 0)?.Result as IUserMessage;

            if (full)
            {
                var text = TextResource.Format(ExceptionText.USER, (user?.Username ?? TBLocalisation.UNKNOWNUSER) + "#" + (user?.Discriminator ?? "0000")) + "\n" +
                           TextResource.Format(ExceptionText.CHANNEL, channel.Name, channel.Id) + "\n";
                if (channel is IGuildChannel guildChannel)
                    text += $"{TextResource.GetResource(ExceptionText.GUILD)}:\n{guildChannel.Guild.Name} ({guildChannel.Guild.Id})\n";
                text += $"{TextResource.GetResource(ExceptionText.MESSAGE)}:\n{message.Content}\n\n";
                text += exception.Content;

                await Reply().WithAttachment(() => text.ToStream(), $"Exception{exceptionId}.txt")
                             .WithMessage(TextResource.Format(ExceptionText.FULLMESSAGE, ReplyType.Success, exceptionId))
                             .SendAsync();
            }
            else
            {
                var builder = new LocalisedEmbedBuilder
                {
                    Timestamp = exception.Time,
                    Color = System.Drawing.Color.Red.ToDiscord()
                }.WithRawDescription(exception.Description)
                 .AddField(f => f.WithName(ExceptionText.MESSAGE).WithRawValue(message.Content))
                 .AddInlineField(f => f.WithName(ExceptionText.CHANNEL).WithRawValue($"{channel.Name} ({channel.Id})"));

                if (user == null)
                    builder.WithAuthor(a => a.WithName(TBLocalisation.UNKNOWNUSER));
                else
                    builder.WithAuthor(a => a.WithIconUrl(user.GetAvatarUrl()).WithRawName($"{user.Username}#{user.Discriminator}"));

                if (channel is IGuildChannel guildChannel)
                    builder.AddInlineField(f => f.WithName(ExceptionText.GUILD).WithRawValue($"{guildChannel.Guild.Name} ({guildChannel.Guild.Id})"));

                await ReplyAsync(builder);
            }
        }
    }
}
