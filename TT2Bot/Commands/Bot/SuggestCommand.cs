using Discord;
using System;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Util;
using TT2Bot.Models;

namespace TT2Bot.Commands.Bot
{
    [Description("Allows you to make suggestions and feature requests for me!")]
    public class SuggestCommand : Command
    {
        [Call]
        [Usage("Sends a suggestion to my home guild.")]
        public async Task SuggestAsync([Dense]string message)
        {
            var settings = SettingsManager.GetCustomGlobal<TT2GlobalSettings>();
            var suggestionChannel = Client.GetChannel(settings.BotSuggestChannel) as IMessageChannel;

            if (suggestionChannel == null)
            {
                await ReplyAsync("I could not find where I need to send the suggestion! Please try again later.", ReplyType.Error);
                return;
            }

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = $"{Author.Username}#{Author.Discriminator}",
                    IconUrl = Author.GetAvatarUrl()
                },
                Timestamp = DateTime.Now,
                Color = System.Drawing.Color.ForestGreen.ToDiscord()
            }
            .AddField("Suggestion", message)
            .AddInlineField(Guild?.Name ?? Author.Username, Guild?.Id ?? Author.Id)
            .AddInlineField(Channel.Name, Channel.Id);
            await suggestionChannel.SendMessageSafeAsync("", embed: builder.Build());
            await ReplyAsync("Suggestion sent", ReplyType.Success);
        }
    }
}
